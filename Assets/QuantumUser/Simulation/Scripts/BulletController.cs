namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class BulletController : SystemMainThreadFilter<BulletController.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public PhysicsBody2D* Body;
            public BulletInfo* Bullet;
        }
        public override void Update(Frame frame, ref Filter filter)
        {
            if(filter.Bullet->Facing == PlayerFacing.Left)
            {
                filter.Body->Velocity = filter.Bullet->Speed * FPVector2.Left;
            }
            else
            {
                filter.Body->Velocity = filter.Bullet->Speed * FPVector2.Right;
            }
        }
    }
}
