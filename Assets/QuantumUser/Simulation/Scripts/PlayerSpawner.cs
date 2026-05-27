using System;
using UnityEngine;
using Quantum;
namespace Quantum
{
    using Photon.Deterministic;
    using System.Collections.Generic;
    using UnityEngine.Scripting;
    using UnityEngine.XR;

    [Preserve]
    public unsafe class PlayerSpawner : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            var playerData = frame.GetPlayerData(player);
            var spawnedPlayer = frame.Create(playerData.PlayerAvatar);
            var playerInfo = frame.Get<PlayerInfo>(spawnedPlayer);
            playerInfo.PlayerRef = player;
            playerInfo.CurrentPlayerRef = player;

            var isLocal = frame.IsPlayerVerifiedOrLocal(player);
            //var playerList ;
            var gameView = frame.GetComponentIterator<LineWiner>();

            foreach (var item in gameView)
            {
                
                var playerList = frame.ResolveList(item.Component.PlayerList);
                playerList.Add(spawnedPlayer);
                frame.Set(item.Entity, item.Component);

                // Set value EntityRef GameManager

                playerInfo.gameManager = item.Entity;
                frame.Set(spawnedPlayer, playerInfo);
            }
            //var playerList = frame.ResolveList(gameView.Pla);

            

            // Setup player vào playerList của global
            var playerListGlobal = frame.ResolveList(frame.Global->playerList);
            playerListGlobal.Add(spawnedPlayer);

            Debug.Log("==============Player List Global=================");
            foreach (var e in playerListGlobal)
            {
                Debug.Log(e);
            }

            for (int i = 0;  i < frame.Global->playerArray.Length; i++)
            {
                if(frame.Global->playerArray[i] == EntityRef.None
                    )
                {
                    frame.Global->playerArray[i] = spawnedPlayer;
                    break;
                }

            }

            // Debug
            Debug.Log("==============Player Array Global=================");
            for (int i = 0; i < frame.Global->playerArray.Length; i++)
            {
                if (frame.Global->playerArray[i] != EntityRef.None)
                {
                    Debug.Log(frame.Global->playerArray[i]);
                }
                else
                {
                    Debug.Log("Null Array" + i);
                }

            }

            if (frame.PlayerConnectedCount == 0)
            {
                playerInfo.PlayerType = PlayerType.Cross;
            }
            else
            {
                playerInfo.PlayerType = PlayerType.Cricle;
                frame.Global->localPlayerType = PlayerType.Cross;
            }

            frame.Set(spawnedPlayer, playerInfo);
        }

        //public void OnPlayerRemoved(Frame frame, PlayerRef player)
        //{
        //    var players = frame.GetComponentIterator<PlayerInfo>();
        //    EntityRef playerEntityRef = EntityRef.None;
        //    foreach(var item in players)
        //    {
        //        if(item.Component.PlayerRef == player)
        //        {
        //            // Remove Player
        //            for(int i = 0; i < frame.Global->playerArray.Length; i++)
        //            {
        //                if (frame.Global->playerArray[i] == item.Entity)
        //                {
        //                    frame.Global->playerArray[i] = EntityRef.None;
        //                }
        //            }
        //            Debug.Log("==============Player Array Global Destroy=================");
        //            for (int i = 0; i < frame.Global->playerArray.Length; i++)
        //            {
        //                if (frame.Global->playerArray[i] != EntityRef.None)
        //                {
        //                    Debug.Log(frame.Global->playerArray[i]);
        //                }
        //                else
        //                {
        //                    Debug.Log("Null Array" + i);
        //                }

        //            }
        //            var playerListGlobal = frame.ResolveList(frame.Global->playerList);
        //            playerListGlobal.Remove(item.Entity);
        //            Debug.Log(playerListGlobal.Count);
        //            Debug.Log("==============Player List Global=================");
        //            foreach (var e in playerListGlobal)
        //            {
        //                Debug.Log(e);
        //            }

        //            frame.Destroy(item.Entity);
        //        }
        //        else
        //        {
        //            // Setup Game 
        //            playerEntityRef = item.Entity;
        //        }
        //    }

        //    if(playerEntityRef != EntityRef.None)
        //    {
        //        PlayerInfo playerInfo = frame.Get<PlayerInfo>(playerEntityRef);
        //        playerInfo.PlayerType = PlayerType.Cross;
        //        frame.Global->localPlayerType = PlayerType.None;
        //        frame.Set(playerEntityRef, playerInfo);
        //    }
        //}
        public void OnPlayerRemoved(Frame frame, PlayerRef player)
        {
            var players = frame.GetComponentIterator<PlayerInfo>();
            EntityRef playerEntityRef = EntityRef.None;

            foreach (var item in players)
            {
                if (item.Component.PlayerRef == player)
                {
                    // 1. Xóa khỏi Global Array
                    for (int i = 0; i < frame.Global->playerArray.Length; i++)
                    {
                        if (frame.Global->playerArray[i] == item.Entity)
                        {
                            frame.Global->playerArray[i] = EntityRef.None;
                        }
                    }

                    // 2. Xóa khỏi Global List
                    var playerListGlobal = frame.ResolveList(frame.Global->playerList);
                    playerListGlobal.Remove(item.Entity);

                    // BỔ SUNG: 3. Xóa khỏi danh sách PlayerList của LineWiner (GameManager)
                    var gameViews = frame.GetComponentIterator<LineWiner>();
                    foreach (var gv in gameViews)
                    {
                        var pList = frame.ResolveList(gv.Component.PlayerList);
                        if (pList.Contains(item.Entity))
                        {
                            pList.Remove(item.Entity);
                            frame.Set(gv.Entity, gv.Component);
                        }
                    }

                    // 4. Tiến hành hủy Entity
                    frame.Destroy(item.Entity);
                }
                else
                {
                    playerEntityRef = item.Entity;
                }
            }

            if (playerEntityRef != EntityRef.None)
            {
                PlayerInfo playerInfo = frame.Get<PlayerInfo>(playerEntityRef);
                playerInfo.PlayerType = PlayerType.Cross;

                // CẨN THẬN: Việc gán None ở đây có thể khiến Client hiện tại không chạy được Update() 
                // Hãy đảm bảo logic localPlayerType của bạn được cập nhật đúng cho máy local đó.
                frame.Global->localPlayerType = PlayerType.Cross;

                frame.Set(playerEntityRef, playerInfo);
            }
        }
    }
}
