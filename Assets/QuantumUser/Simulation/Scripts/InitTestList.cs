namespace Quantum
{
    using Photon.Deterministic;
    using System;
    using UnityEngine.Scripting; // để dùng [Preserve]

    [Preserve] // đảm bảo không bị strip khi build
    public unsafe class InitTestList : SystemMainThread,
        ISignalOnComponentAdded<LineWiner>, ISignalOnComponentRemoved<LineWiner>
        //, ISignalOnComponentAdded<CollisionAttackPlayer>, ISignalOnComponentRemoved<CollisionAttackPlayer>
    {
        public override void Update(Frame frame)
        {
            if (!frame.Global->isStartGameList)
            {
                frame.Global->playerList = frame.AllocateList<EntityRef>();
                frame.Global->isStartGameList = true;
            }
        }

        public void OnAdded(Frame frame, EntityRef entity, LineWiner* component)
        {
            // Cấp bộ nhớ cho list khi component Test được thêm vào entity
            component->PlayerList = frame.AllocateList<EntityRef>();
            component->valueList = frame.AllocateList<int>();
            
        }

        public void OnAdded(Frame frame, int value, LineWiner* component)
        {
            // Cấp bộ nhớ cho list khi component Test được thêm vào entity
            component->valueList = frame.AllocateList<int>();

        }

        public void OnRemoved(Frame frame, EntityRef entity, LineWiner* component)
        {
            // Giải phóng list khi component Test bị xóa
            frame.FreeList(component->PlayerList);
            component->PlayerList = default;
            frame.FreeList(component->valueList);
            component->valueList = default;
        }

        public void OnRemoved(Frame frame, int value, LineWiner* component)
        {
            // Giải phóng list khi component Test bị xóa
            frame.FreeList(component->valueList);
            component->valueList = default;
        }
    }
}




// Comment Code
//public void OnAdded(Frame frame, EntityRef entity, CollisionAttackPlayer* component)
//{
//    // Cấp bộ nhớ cho list khi component Test được thêm vào entity
//    component->ListEnemy = frame.AllocateList<EntityRef>();
//}

//public void OnRemoved(Frame frame, EntityRef entity, CollisionAttackPlayer* component)
//{
//    // Giải phóng list khi component Test bị xóa
//    frame.FreeList(component->ListEnemy);
//    component->ListEnemy = default;
//}
