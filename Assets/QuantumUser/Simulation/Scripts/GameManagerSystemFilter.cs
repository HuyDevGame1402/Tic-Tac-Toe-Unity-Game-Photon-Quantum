namespace Quantum
{
    using Photon.Deterministic;
    using System;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameManagerSystemFilter : SystemMainThreadFilter<GameManagerSystemFilter.Filter>, ISignal
    {
        public struct Filter
        {
            public EntityRef Entity;
            public LineWiner* GameManagerInfo;
        }

        public override void OnInit(Frame frame)
        {
            // Lấy tất cả các entity có component LineWiner
            var it = frame.GetComponentIterator<LineWiner>();

            foreach (var e in it)
            {
                var lineWiner = e.Component;
                var valueListManager = frame.ResolveList(lineWiner.valueList);
                for (int i = 0; i < 9; i++)
                {
                    valueListManager.Add(0);
                }

                frame.Set(e.Entity, lineWiner);
            }
        }


        public override void Update(Frame frame, ref Filter filter)
        {

            var playerList = frame.ResolveList(filter.GameManagerInfo->PlayerList);

            // check tie

            var playerInfo1 = frame.Get<PlayerInfo>(playerList[0]);
            var listValuePlayer1 = playerInfo1.Value;

            var playerInfo2 = frame.Get<PlayerInfo>(playerList[1]);
            var listValuePlayer2 = playerInfo2.Value;

            bool player1Win = HasThreeInRow(listValuePlayer1);
            bool player2Win = HasThreeInRow(listValuePlayer2);
            bool checkFullBoard = CheckFullBoard(listValuePlayer1, listValuePlayer2);

            if (!player1Win && !player2Win && checkFullBoard)
            {
                frame.Global->winIndex = 3;
                frame.Signals.OnPlayerTie();
            }
        }

        private bool CheckFullBoard(int* values1, int* values2)
        {
            for(int i = 0; i < 8; i++)
            {
                if (values1[i] == 0 && values2[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool HasThreeInRow(int* values)
        {
            int[][] winPatterns = new int[][]
            {
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 3, 6 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 0, 4, 8 },
                new int[] { 2, 4, 6 }
            };

            for (int i = 0; i < winPatterns.Length; i++)
            {
                var pattern = winPatterns[i];
                if (values[pattern[0]] == values[pattern[1]] &&
                    values[pattern[1]] == values[pattern[2]]
                    )
                {
                    return true;
                }
            }
            return false;
        }
    }
}
