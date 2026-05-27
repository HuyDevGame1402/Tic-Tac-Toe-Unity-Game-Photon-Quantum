namespace Quantum
{
    using Photon.Deterministic;
    using System;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameController : SystemSignalsOnly, ISignalOnPlayerWin, ISignalOnPlayerLose, 
        ISignalOnPlayerResetGame, ISignalOnPlayerTie, ISignalOnPlayerMove
    {
        public void OnPlayerLose(Frame frame)
        {
            frame.Global->CurrentGameState = GameState.Lose;
        }
        public void OnPlayerWin(Frame frame)
        {
            frame.Global->CurrentGameState = GameState.Win;
        }
        public void OnPlayerResetGame(Frame frame)
        {
            frame.Global->CurrentGameState = GameState.Reset;
        }
        public void OnPlayerTie(Frame frame)
        {
            frame.Global->CurrentGameState = GameState.Tie;
        }

        public void OnPlayerMove(Frame frame, EntityRef gameManager, int value, int pos)
        {
            var gameManagerInfo = frame.Get<LineWiner>(gameManager);
            var valueListManager = frame.ResolveList(gameManagerInfo.valueList);

            if (valueListManager[pos] != 0 && pos < valueListManager.Count)
            {
                valueListManager[pos] = value;
            }
        }
    }
}
