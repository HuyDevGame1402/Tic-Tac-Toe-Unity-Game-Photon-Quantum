namespace Quantum
{
    using UnityEngine;
    using System;

    public class CheckPlayerGameState : QuantumEntityViewComponent
    {
        PlayerInfo playerInfo;
        bool isPause = false;
        private void Update()
        {
            if (VerifiedFrame.Has<PlayerInfo>(_entityView.EntityRef))
            {
                playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
                if (QuantumRunner.Default.Game.PlayerIsLocal(playerInfo.PlayerRef) && playerInfo.isPlaying && !isPause)
                {
                    Debug.Log("---------------------------On Signal GameWin------------------------------- ");
                    //VerifiedFrame.Signals.OnPlayerWin();
                    isPause = true;
                }
            }         
        }
    }
}
