namespace Quantum
{
    using Photon.Deterministic;
    using Spine.Unity;
    using UnityEngine;
    public enum AnimationName
    {
        Idle, Walk, Attack
    }
    public unsafe class PlayerView : QuantumEntityViewComponent
    {
        //public const string ANIM_IDLE = "Idle";
        //public const string ANIM_WALK = "Walk";
        //public const string ANIM_ATTACK = "Attack";
        //public Quaternion rotationLeft;
        //public Quaternion rotationRight;
        //private Animator animator;
        //private PhysicsBody2D body;
        //private AnimationName currentAnim;
        //private PlayerInfo playerInfo;

        //public bool spawnBullet = false;
        //private void Awake()
        //{
        //    animator = GetComponentInChildren<Animator>();
        //    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        //}
        //private void Update()
        //{
        //    currentAnim = GetCurrentAnimation();
        //    // giải thích đơn giản: _entityView thường đc gắn vào gameobject đại diện cho gameobject ví dụ player hay vũ khí
        //    // Khác với PlayerRef thì EntityRef đại diện cho player hoặc vũ khí có nhiều EntityRef 
        //    // Còn PlayerRef đại diện cho ng chs thì ng chs gồm player vũ khí, .....
        //    // _entityView gắn vào player trỏ tới EntityRef sẽ đc người player hiện tại tại sao k get lấy luôn PhysicsBody2D
        //    // vì PhysicsBody2D của vũ khí thì sao nên p đặt hẳn ra _entityView.EntityRef
        //    body = VerifiedFrame.Get<PhysicsBody2D>(_entityView.EntityRef);
        //    if(body.Velocity != FPVector2.Zero && currentAnim != AnimationName.Attack)
        //    {
        //        animator.Play(ANIM_WALK);
        //    }
        //    playerInfo = VerifiedFrame.Get<PlayerInfo>(_entityView.EntityRef);
        //    var input = VerifiedFrame.GetPlayerInput(playerInfo.PlayerRef);
        //    if (input->Attack.WasPressed)
        //    {
        //        animator?.Play(ANIM_ATTACK);
        //    }
        //    if (body.Velocity.X > 0)
        //    {
        //        animator.transform.rotation = rotationRight;
        //    }
        //    else if(body.Velocity.X < 0)
        //    {
        //        animator.transform.rotation = rotationLeft;
        //    }
        //}
        //private AnimationName GetCurrentAnimation()
        //{
        //    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //    if(stateInfo.IsName(ANIM_ATTACK)) return AnimationName.Attack;
        //    else if(stateInfo.IsName(ANIM_WALK)) return AnimationName.Walk;
        //    return AnimationName.Idle;
        //}
        //public void PollInput(CallbackPollInput callback)
        //{
        //    // vì có nhiều ng chs cần lấy ng chs hiện tại 
        //    if (QuantumRunner.DefaultGame.PlayerIsLocal(playerInfo.PlayerRef) ==  false)
        //    {
        //        return;
        //    }
        //    Quantum.Input input = new Quantum.Input();
        //    FP x, y;
        //    GetPlayerInputDirection(out x, out y);
        //    input.Direction = new FPVector2(x, y);
        //    input.Attack = UnityEngine.Input.GetKey(KeyCode.Space);
        //    input.SpawnBullet = spawnBullet;
        //    if(spawnBullet == true) spawnBullet = false;   
        //    callback.SetInput(input, DeterministicInputFlags.Repeatable);
        //}
        //private void GetPlayerInputDirection(out FP x, out FP y)
        //{
        //    x = 0;
        //    y = 0;
        //    if(currentAnim == AnimationName.Attack)
        //    {
        //        return;
        //    }
        //    if (UnityEngine.Input.GetKey(KeyCode.RightArrow))
        //    {
        //        x = 1;
        //    }
        //    else if (UnityEngine.Input.GetKey(KeyCode.LeftArrow))
        //    {
        //        x = -1;
        //    }
        //    if (UnityEngine.Input.GetKey(KeyCode.UpArrow))
        //    {
        //        y = 1;
        //    }
        //    else if (UnityEngine.Input.GetKey(KeyCode.DownArrow))
        //    {
        //        y = -1;
        //    }
        //}
    }
}
