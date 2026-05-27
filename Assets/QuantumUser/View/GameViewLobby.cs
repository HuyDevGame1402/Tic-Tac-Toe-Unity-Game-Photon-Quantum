namespace Quantum
{
    using Quantum.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public unsafe class GameViewLobby : QuantumEntityViewComponent
    {
        [SerializeField] private Transform slotTransform1;
        [SerializeField] private Transform slotTransform2;
        [SerializeField] private Transform buttonStartGame;
        public int _lastPlayerCount = -1;

        [SerializeField] private Transform transformLine;
        [SerializeField] private Transform gameManagerVisual;
        [SerializeField] private Transform uiGame;
        [SerializeField] private Transform lobbyTransform;

        [SerializeField] private Transform lobbyUI;

        [SerializeField] private Sprite iconDelete;
        [SerializeField] private Sprite iconSwap;

        public int coutnPlayer;
        public int currentCount;

        private void Update()
        {
            if (VerifiedFrame == null) return;
            if (!VerifiedFrame.Unsafe.TryGetPointer(EntityRef, out LineWiner* lineWiner))
                return;

            var playerList = VerifiedFrame.ResolveList(VerifiedFrame.Global->playerList);
            var playerArray = VerifiedFrame.Global->playerArray;
            currentCount = playerList.Count;

            if (currentCount != _lastPlayerCount)
            {
                _lastPlayerCount = currentCount;
                OnPlayerListChanged(playerList);
            }

            if (VerifiedFrame.Global->isInitGame/* && currentCount == 2*/)
            {
                Debug.Log("Chay Game 2");
                OnInitGame();
            }
        }
        private void OnPlayerListChanged(QList<EntityRef> players)
        {
            if(players.Count == 0)
            {
                return;
            }
            buttonStartGame.gameObject.SetActive(false);
            var playerArray = VerifiedFrame.Global->playerArray;
            SetActiveIconDelte(false, EntityRef.None, iconDelete);
            for (int i = 0; i < playerArray.Length; i++)
            {
                if(playerArray[i] != EntityRef.None)
                {
                    //lobbyUI.GetChild(i).gameObject.SetActive(true);
                    SetUpAvtAndName(lobbyUI.GetChild(i),true);
                    var playerInfoLocal = VerifiedFrame.Get<PlayerInfo>(playerArray[i]);
                    if (QuantumRunner.Default.Game.Session.IsLocalPlayer(playerInfoLocal.PlayerRef))
                    {
                        lobbyUI.GetChild(i).Find("BackgroundLocal").gameObject.SetActive(true);
                    }
                }
                else
                {
                    //lobbyUI.GetChild(i).gameObject.SetActive(false);
                    SetUpAvtAndName(lobbyUI.GetChild(i), false);
                    lobbyUI.GetChild(i).Find("BackgroundLocal").gameObject.SetActive(false);
                }
            }

            // Setup Button Start Game (chỉ host hay server tức người client đầu tiên hoặc sớm nhất trong team)
            var playerInfo = VerifiedFrame.Get<PlayerInfo>(players[0]);
            if (QuantumRunner.Default.Game.Session.IsLocalPlayer(playerInfo.PlayerRef))
            {
                buttonStartGame.gameObject.SetActive(true);
                SetActiveIconDelte(true, players[0], iconDelete);
            }
            else
            {
                SetIconSawpClient(iconSwap);
            }
            Debug.Log("Chay Xong!");
        }

        private void OnInitGame()
        {
           transformLine.gameObject.SetActive(true);
           gameManagerVisual.gameObject.SetActive(true);
           uiGame.gameObject.SetActive(true);
           lobbyTransform.gameObject.SetActive(false);
        }

        private void SetActiveIconDelte(bool isActive, EntityRef playerEntityRef, Sprite iconDelte)
        {
            if (!isActive)
            {
                for (int i = 0; i < lobbyUI.childCount; i++)
                {
                    lobbyUI.GetChild(i).Find("ImageIcon").gameObject.SetActive(isActive);
                }
            }
            else
            {
                int indexSlot = GetIndexSLot(playerEntityRef);
                for (int i = 0; i < lobbyUI.childCount; i++)
                {
                    if(i == indexSlot)
                    {
                        continue;
                    }
                    if (lobbyUI.GetChild(i).Find("ImageAvt").gameObject.activeSelf)
                    {
                        lobbyUI.GetChild(i).Find("ImageIcon").gameObject.SetActive(isActive);
                        lobbyUI.GetChild(i).Find("ImageIcon").GetComponent<Image>().sprite = iconDelte;
                    }
                }
            }
        }

        private int GetIndexSLot(EntityRef playerEntityRef)
        {
            var playerArray = VerifiedFrame.Global->playerArray;
            for(int i = 0; i < playerArray.Length; i++)
            {
                if (playerArray[i] == EntityRef.None) continue;
                if(playerArray[i] == playerEntityRef)
                {
                    return i;
                }
            }

            return -1;
        }

        private void SetIconSawpClient(Sprite iconSwap)
        {
            var playerArray = VerifiedFrame.Global->playerArray;
            for (int i = 0; i < playerArray.Length; i++)
            {
                if (playerArray[i] != EntityRef.None)
                {
                    lobbyUI.GetChild(i).Find("ImageIcon").gameObject.SetActive(false);
                    continue;
                }
                if (playerArray[i] == EntityRef.None)
                {
                    lobbyUI.GetChild(i).Find("ImageIcon").gameObject.SetActive(true);
                    lobbyUI.GetChild(i).Find("ImageIcon").GetComponent<Image>().sprite = iconSwap;
                }
            }
        }
        private void SetUpAvtAndName(Transform slot , bool isActive)
        {
            slot.Find("ImageAvt").gameObject.SetActive(isActive);
            slot.Find("TextName").gameObject.SetActive(isActive);
        }
    }
}
