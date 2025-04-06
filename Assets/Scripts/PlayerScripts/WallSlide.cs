using UnityEngine;

namespace PlayerScripts
{
    public class WallSlide : PlayerState
    {
        protected internal WallSlide(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Player.Rb.linearVelocity = new Vector2(0, Player.Rb.linearVelocity.y);
        }

        public override void Update()
        {
            base.Update();

            if (Input.GetButtonDown("Jump"))
                StateMachine.ChangeState(StateMachine.GetState<WallJump>());
            else if (!Player.OnTheWall || Player.IsGrounded)
                StateMachine.ChangeState(StateMachine.GetState<PlayerGroundedState>());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (Movement != Vector2.zero) Player.Rb.AddForce(Movement * Vector2.up, ForceMode2D.Force);
            else Player.Rb.linearVelocity = Vector2.down * 5;
            
            /*if (Player.Rb.linearVelocity.magnitude > 0)
            {
                //if (!Player.WallParticles.isPlaying) Player.WallParticles.Play();
                //if (!Player.WallSparkRenderer.enabled) Player.WallSparkRenderer.enabled = true;
            }
            else
            {
                //if (Player.WallParticles.isPlaying) Player.WallParticles.Stop();
                //if (Player.WallSparkRenderer.enabled) Player.WallSparkRenderer.enabled = false;
            }*/
        }

        public override void Exit()
        {
            base.Exit();
            //if (Player.WallParticles.isPlaying) Player.WallParticles.Stop();
            //if (Player.WallSparkRenderer.enabled) Player.WallSparkRenderer.enabled = false;
        }
    }
}
