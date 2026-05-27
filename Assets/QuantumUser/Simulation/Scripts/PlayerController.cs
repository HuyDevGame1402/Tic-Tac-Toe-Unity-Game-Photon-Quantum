using Quantum;

namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Windows;
    using System;
    using System.Collections;

    [Preserve]
    public unsafe class PlayerController : SystemMainThreadFilter<PlayerController.Filter>, ISignal
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerInfo* PlayerInfo;
        }
        string direction = "";
        public static event Action<EntityRef> AddList;
        private List<EntityRef> entityRefs = new List<EntityRef>();
        public FP timer = 1;
        public override void Update(Frame frame, ref Filter filter)
        {
            var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            if (filter.PlayerInfo->PlayerRef.IsValid && input->NewGame == false)
            {
                if (input->CreateProtoype && filter.PlayerInfo->PlayerType == frame.Global->localPlayerType)
                {
                    if (filter.PlayerInfo->Value[input->PosArray] == 0)
                    {
                        EntityRef spawner;
                        int value = (filter.PlayerInfo->PlayerType == PlayerType.Cross) ? 1 : 2;
                        spawner = frame.Create(filter.PlayerInfo->PlayerType == PlayerType.Cross ? filter.PlayerInfo->Cross : filter.PlayerInfo->Cricle);
                        filter.PlayerInfo->Value[input->PosArray] = value;

                        var playerInfos = frame.GetComponentIterator<PlayerInfo>();
                        foreach (var playerInfo in playerInfos)
                        {
                            if (playerInfo.Component.Value[input->PosArray] == 0)
                            {
                                var updatedComponent = playerInfo.Component;
                                updatedComponent.Value[input->PosArray] = value;
                                frame.Set(playerInfo.Entity, updatedComponent); // ✅ Lưu thay đổi vào frame
                            }
                        }

                        // Cập nhật vị trí object trong Quantum
                        var transform = frame.Get<Transform2D>(spawner);
                        transform.Position = input->PositionCreatePrototype;
                        frame.Set(spawner, transform);
                        entityRefs.Add(spawner);
                        AddList?.Invoke(spawner);
                        frame.Global->localPlayerType = filter.PlayerInfo->PlayerType == PlayerType.Cross ? PlayerType.Cricle : PlayerType.Cross;
                        input->CreateProtoype = false;
                        // Chuyển đổi mảng con trỏ thành mảng trước khi gọi CheckWin
                        int[] boardArray = ConvertPointerToArray(filter.PlayerInfo->Value, 9);
                        int winIndex = CheckWin(boardArray);

                        frame.Signals.OnPlayerMove(filter.PlayerInfo->gameManager, value, input->PosArray);

                        if (winIndex != -1)
                        {
                            switch (direction)
                            {
                                case "Horizontal":
                                    frame.Global->rotation = 0;
                                    break;
                                case "Vertical":
                                    frame.Global->rotation = 90;
                                    break;
                                case "DiagonalRight":
                                    frame.Global->rotation = 45;
                                    break;
                                case "DiagonalLeft":
                                    frame.Global->rotation = 135;
                                    break;
                                default:
                                    break;
                            }
                            direction = "";

                            Debug.Log($"Player {value} wins! Middle index: {winIndex}");
                            frame.Global->winIndex = winIndex;
                            if (value == 1)
                            {
                                // Cross win
                                frame.Global->valueWiner = 1;
                                frame.Signals.OnPlayerWin();
                            }
                            else if (value == 2)
                            {
                                // Cricle win
                                frame.Global->valueWiner = 2;
                                frame.Signals.OnPlayerWin();
                            }
                            
                            

                            return;
                        }
                        
                    }
                }
                
            }
            //Debug.Log($"NewGame Value: {input->NewGame}");
            if (input->NewGame)
            {
                var playerInfos = frame.GetComponentIterator<PlayerInfo>();
                Debug.Log("ResetGame Part3!");
                if(entityRefs.Count > 0)
                {
                    for (int i = 0; i < entityRefs.Count; i++)
                    {
                        frame.Destroy(entityRefs[i]);
                    }
                }
                //entityRefs.Clear();
                // Sau khi xóa, cập nhật dữ liệu
                foreach (var playerInfo in playerInfos)
                {
                    unsafe
                    {
                        var updatedComponent = playerInfo.Component; // Sao chép struct
                        for (int i = 0; i < 9; i++)
                        {
                            *(updatedComponent.Value + i) = 0; // ✅ Gán giá trị vào con trỏ
                        }
                        frame.Set(playerInfo.Entity, updatedComponent); // Cập nhật lại vào frame
                    }
                }
                frame.Signals.OnPlayerResetGame();

                if(frame.Global->winIndex == 1)
                {
                    frame.Global->localPlayerType = PlayerType.Cricle;
                    frame.Global->scoreCross += 1;
                }
                else if(frame.Global->winIndex == 2)
                {
                    frame.Global->localPlayerType = PlayerType.Cross;
                    frame.Global->scoreCircle += 1;
                } 
            }

            if(frame.Global->isInitGame == true)
            {
                timer -= frame.DeltaTime;
                if(timer < 0)
                {
                    frame.Global->isInitGame = false;
                }
            }
            if (input->InitGame.WasPressed)
            {
                frame.Global->isInitGame = true;
            }


        }

        
        private int CheckWin(int[] board)
        {
            int[][] winPatterns = new int[][]
            {
                new int[] { 0, 1, 2 }, // Hàng ngang 1
                new int[] { 3, 4, 5 }, // Hàng ngang 2
                new int[] { 6, 7, 8 }, // Hàng ngang 3
                new int[] { 0, 3, 6 }, // Cột dọc 1
                new int[] { 1, 4, 7 }, // Cột dọc 2
                new int[] { 2, 5, 8 }, // Cột dọc 3
                new int[] { 0, 4, 8 }, // Chéo chính
                new int[] { 2, 4, 6 }  // Chéo phụ
            };
            string[] patternNames = new string[]
            {
                    "Horizontal", "Horizontal", "Horizontal", // Ba hàng ngang
                    "Vertical", "Vertical", "Vertical",       // Ba cột dọc
                    "DiagonalLeft",                         // Chéo chính
                    "DiagonalRight"                           // Chéo phụ
            };
            for(int i = 0; i < winPatterns.Length; i++)
            {
                var pattern = winPatterns[i];
                if (board[pattern[0]] != 0 && board[pattern[0]] == board[pattern[1]] && board[pattern[1]] == board[pattern[2]])
                {
                    SetDirection(patternNames[i]);
                    return pattern[1]; // Trả về index giữa
                }
            }
            // Check tie

            

            return -1; // Không ai thắng
        }
        // Chuyển đổi con trỏ int* thành mảng int[]
        private int[] ConvertPointerToArray(int* ptr, int length)
        {
            int[] array = new int[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = ptr[i];
            }
            return array;
        }
        private void SetDirection(String direction)
        {
            this.direction = direction;
        }
    }
}


//---------------------------Update Old--------------------------------
//public override void Update(Frame frame, ref Filter filter)
//{
//    if (filter.PlayerInfo->PlayerRef.IsValid)
//    {
//        var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
//        if (input->CreateProtoype)
//        {
//            EntityRef spawner;
//            if (filter.PlayerInfo->PlayerType == PlayerType.Cross)
//            {
//                spawner = frame.Create(filter.PlayerInfo->Cross);
//                board[input->x.AsInt, input->y.AsInt] = 1;
//                filter.PlayerInfo->ValueBoard = 1;
//            }
//            else
//            {
//                spawner = frame.Create(filter.PlayerInfo->Cricle);
//                board[input->x.AsInt, input->y.AsInt] = 2;
//                filter.PlayerInfo->ValueBoard = 2;
//            }
//            var transform = frame.Get<Transform2D>(spawner);
//            transform.Position = input->PositionCreatePrototype;
//            frame.Set(spawner, transform);

//            input->CreateProtoype = false;
//        }
//    }
//    else
//    {
//        Debug.Log("PlayerRef null!");
//    }
//}

// -------------- Lấy OtherPlayerRef--------------------
//if (!filter.PlayerInfo->OrtherPlayerRef.IsValid)
//{
//    var players = frame.GetComponentIterator<PlayerInfo>();
//    PlayerRef currentPlayerRef = filter.PlayerInfo->PlayerRef;
//    foreach (var p in players)
//    {
//        if (p.Component.PlayerRef != currentPlayerRef)
//        {
//            filter.PlayerInfo->OrtherPlayerRef = p.Component.PlayerRef;
//            break;
//        }
//    }
//}

//------------------------------------------------------------
//filter.Body->Velocity = input->Direction * filter.PlayerInfo->Speed;
//if(input->Direction.X > 0)
//{
//    filter.PlayerInfo->Facing = PlayerFacing.Right;
//}
//else if (input->Direction.X < 0)
//{
//    filter.PlayerInfo->Facing = PlayerFacing.Left;
//}
//if (input->SpawnBullet)
//{
//    var spawnedBullet = frame.Create(filter.PlayerInfo->Bullet);
//    var transform = frame.Get<Transform2D>(spawnedBullet);
//    transform.Position = filter.Transform->Position;
//    var bulletInfo = frame.Get<BulletInfo>(spawnedBullet);
//    bulletInfo.Owner = filter.Entity;
//    bulletInfo.Facing = filter.PlayerInfo->Facing;
//    frame.Set(spawnedBullet, transform);
//    frame.Set(spawnedBullet, bulletInfo);
//}