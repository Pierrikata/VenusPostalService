using UnityEngine;

namespace PlayerScripts
{
    public class PlayerGroundedState : PlayerState
    {
        protected internal PlayerGroundedState(Player player, PlayerStateMachine stateMachine)
            : base(player, stateMachine)
        {
        }

        public override void Exit()
        {
            base.Exit();
            //if (Player.trailRenderer.emitting) Player.trailRenderer.emitting = false;
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetButtonDown("Jump"))
                StateMachine.ChangeState(StateMachine.GetState<Jump>());
            else if (!Player.IsGrounded)
                StateMachine.ChangeState(StateMachine.GetState<FreeFall>());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            /*if (Player.Rb.linearVelocity.magnitude > 31.49 && Input.GetAxisRaw("Horizontal") != 0)
            {
                //if (BoosterSfxSource.clip != Player.soundData[2].clip) BoosterSfxSource.clip = Player.soundData[2].clip;
                //if (!BoosterSfxSource.isPlaying) BoosterSfxSource.Play();
                //if (!Player.trailRenderer.emitting) Player.trailRenderer.emitting = true;
            }
            else
            {
                //if (BoosterSfxSource.isPlaying) BoosterSfxSource.Stop();
                //if (Player.trailRenderer.emitting) Player.trailRenderer.emitting = false;
            }*/
        }

        protected override void Locomotion()
        {
            base.Locomotion();
            
            // applies force to rigid body, multiplying by Vector2.right so that it only affects X axis
            Player.Rb.AddForce(Movement * Vector2.right, ForceMode2D.Force);
        }
    }
}

