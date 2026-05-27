namespace Quantum
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections.Generic;

    public unsafe class GameView : QuantumEntityViewComponent
    {
        public UnityEngine.UI.Image arrowCross;
        public UnityEngine.UI.Image arrowCricle;
        public TextMeshProUGUI textMeshProUGUI;
        
        public static bool isPlaying = true;
        [SerializeField] private List<EntityRef> list = new List<EntityRef>();
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private Transform lineWinerPrefab;
        [SerializeField] private Color colorWhite;
        [SerializeField] private Color colorBlack;
        [SerializeField] private TextMeshProUGUI textScoreCross;
        [SerializeField] private TextMeshProUGUI textScoreCricle;
        [SerializeField] private int scoreCroos = 0;
        [SerializeField] private int scoreCricle = 0;
        [SerializeField] private Color colorWinerCross;
        [SerializeField] private Color colorWinerCircle;
        [SerializeField] private Color colorTie;
        private Transform spawnPrefab;
        private void Awake()
        {
            PlayerController.AddList += GameView_AddList;
            //button.onClick.AddListener(SignalReset);
            Hide();
        }
            private void Update()
            {
                if (VerifiedFrame == null) return;

                if (VerifiedFrame.Global->localPlayerType == PlayerType.None)
                {
                    return;
                }
                if (isPlaying)
                {
                    if (VerifiedFrame.Global->localPlayerType == PlayerType.Cross)
                    {
                        ShowArrowCross();
                    }
                    else if (VerifiedFrame.Global->localPlayerType == PlayerType.Cricle)
                    {
                        ShowArrowCricle();
                    }
                    if (VerifiedFrame.Global->CurrentGameState == GameState.Win && !textMeshProUGUI.gameObject.activeSelf)
                    {
                        if (!VerifiedFrame.Exists(_entityView.EntityRef))
                        {
                            Debug.LogWarning($"Entity {_entityView.EntityRef} đã bị hủy, không thể lấy dữ liệu LineWiner!");
                            return;
                        }
                    var gameViewInfo = VerifiedFrame.Get<LineWiner>(_entityView.EntityRef);
                        var playerList = VerifiedFrame.ResolveList(gameViewInfo.PlayerList);
                        if (VerifiedFrame.Global->valueWiner == 1)
                        {
                            //if(playerInfo.PlayerType == PlayerType.Cross)
                            //{
                            //    textMeshProUGUI.text = "You Win!";
                            //}
                            //textMeshProUGUI.text = "Cross Win!";

                            for(int i = 0; i < playerList.Count; i++)
                                {
                                    var playerEntity = playerList[i];
                                    if (!VerifiedFrame.Exists(playerEntity)) continue;
                                    var playerInfo = VerifiedFrame.Get<PlayerInfo>(playerEntity);
                                    if (QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef))
                                    {
                                        if(playerInfo.PlayerType == PlayerType.Cross)
                                        {
                                            textMeshProUGUI.text = "You Win!";
                                            textMeshProUGUI.color = colorWinerCross;
                                        }
                                        else
                                        {
                                            textMeshProUGUI.text = "You Loss!";
                                            textMeshProUGUI.color = colorWinerCircle;
                                        }
                                    }
                                }

                        //textMeshProUGUI.color = colorWinerCross;
                            scoreCroos += 1;
                            textScoreCross.text = (scoreCroos).ToString();
                        }
                        else if (VerifiedFrame.Global->valueWiner == 2)
                        {
                        //if(playerInfo.PlayerType == PlayerType.Cricle)
                        //{
                        //    textMeshProUGUI.text = "You";
                        //}
                        for (int i = 0; i < playerList.Count; i++)
                        {
                            var playerEntity = playerList[i];
                            var playerInfo = VerifiedFrame.Get<PlayerInfo>(playerEntity);
                            if (QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef))
                            {
                                if (playerInfo.PlayerType == PlayerType.Cross)
                                {
                                    textMeshProUGUI.text = "You Loss!";
                                    textMeshProUGUI.color = colorWinerCross;
                                }
                                else
                                {
                                    textMeshProUGUI.text = "You Win!";
                                    textMeshProUGUI.color = colorWinerCircle;
                                }
                            }
                        }
                            scoreCricle += 1;
                            textScoreCricle.text = (scoreCricle).ToString();
                        }
                        textMeshProUGUI.gameObject.SetActive(true);
                        button.gameObject.SetActive(true);
                        Vector2 positionLineWiner = GetVector2LineWiner(VerifiedFrame.Global->winIndex);
                        Vector3 newRorationLineWiner = GetRorationLineWiner(VerifiedFrame.Global->rotation);
                        if(spawnPrefab == null)
                        {
                            spawnPrefab = Instantiate(lineWinerPrefab, positionLineWiner, Quaternion.Euler(newRorationLineWiner));
                        }
                        else
                        {
                            spawnPrefab.position = positionLineWiner;
                            spawnPrefab.gameObject.SetActive(true);
                            spawnPrefab.rotation = Quaternion.Euler(newRorationLineWiner);
                            
                        }
                        isPlaying = false;
                    }
                    if (VerifiedFrame.Global->CurrentGameState == GameState.Tie && !textMeshProUGUI.gameObject.activeSelf)
                    {
                        textMeshProUGUI.text = "You Tie!";
                        textMeshProUGUI.color = colorTie;
                        textMeshProUGUI.gameObject.SetActive(true);
                        button.gameObject.SetActive(true);
                        isPlaying = false;
                    }

            }
                else 
                {
                    if(VerifiedFrame.Global->CurrentGameState == GameState.Reset)
                    {
                        button.gameObject.SetActive(false);
                        textMeshProUGUI.gameObject.SetActive(false);
                        if(spawnPrefab != null)
                        {
                            spawnPrefab.gameObject.SetActive(false);
                        }
                        //spawnPrefab.gameObject.SetActive(false);
                        isPlaying = true;
                    }
                }

            }
        private Vector2 GetVector2LineWiner(int middleIndex)
        {
            switch(middleIndex)
            {
                case 1:
                    return new Vector2(0, 3.5f);
                case 3:
                    return new Vector2(-3.5f, 0);
                case 4:
                    return new Vector2(0, 0);
                case 5:
                    return new Vector2(3.5f, 0);
                case 7:
                    return new Vector2(0, 3.5f);
                default:
                    return Vector2.zero;
            }
        }
        private Vector3 GetRorationLineWiner(int rotationZ)
        {
            switch (rotationZ)
            {
                case 0:
                    return new Vector3(0, 0, 0);
                case 90:
                    return new Vector3(0, 0, 90);
                case 45:
                    return new Vector3(0, 0, 45);
                case 135:
                    return new Vector4(0, 0, 135);
                default:
                    return Vector3.zero;
            }
        }
        private void Hide()
        {
            //arrowCross.gameObject.SetActive(false);
            //arrowCricle.gameObject.SetActive(false);
            textMeshProUGUI.gameObject.SetActive(false);
            button.gameObject.SetActive(false);
        }
        private void ShowArrowCross()
        {
            //arrowCross.gameObject.SetActive(true);
            //arrowCricle.gameObject.SetActive(false);
            arrowCross.color = colorWhite;
            arrowCricle.color = colorBlack;
        }
        private void ShowArrowCricle()
        {
            //arrowCross.gameObject.SetActive(false);
            //arrowCricle.gameObject.SetActive(true);
            arrowCross.color = colorBlack;
            arrowCricle.color = colorWhite;
        }
        private void GameView_AddList(EntityRef entityRef)
        {
            list.Add(entityRef);
            Debug.Log(entityRef.ToString());
        }
    }
}
