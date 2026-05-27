namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class GameVisualManager : SystemMainThreadFilter<GameVisualManager.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            //public GameManager* GameManager;
            public PlayerInfo* PlayerInfo;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            //var input = frame.GetPlayerInput(filter.PlayerInfo->PlayerRef);
            //if (input->OnCreateProtptype)
            //{
            //    Debug.Log(input->x.ToString() + "," + input->y.ToString());
            //    input->OnCreateProtptype = false;
            //}
        }
        private void OnCreatePrototype()
        {
            Debug.Log("[Quantum] CreatePrototype được gọi trong GameVisualManager.");
            // Xử lý logic khi sự kiện được gọi, ví dụ tạo một thực thể mới hoặc kích hoạt hiệu ứng
        }
    }
}
