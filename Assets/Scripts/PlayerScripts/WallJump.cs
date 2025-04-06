using UnityEngine;

namespace PlayerScripts
{
    public class WallJump : PlayerState
    {
        protected internal WallJump(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            StateTimer = .5f;
        
            // Apply force in opposite direction of wall
            var wallJumpForce = Vector2.one * Player.jumpScale;
            wallJumpForce.x *= -Player.FacingDir;
            wallJumpForce.y *= 1.5f;

            if (!Mathf.Approximately(Mathf.Sign(Player.Rb.linearVelocity.x), Mathf.Sign(wallJumpForce.x)))
                wallJumpForce.x -= Player.Rb.linearVelocity.x;

            Player.Rb.AddForce(wallJumpForce * Player.wallJumpAmp, ForceMode2D.Impulse);
            Player.Flip();
        }

        public override void Update()
        {
            base.Update();
            if (StateTimer < 0) StateMachine.ChangeState(StateMachine.GetState<FreeFall>());
        }
    }
}
