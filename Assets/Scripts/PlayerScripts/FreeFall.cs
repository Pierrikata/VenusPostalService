using UnityEngine;

namespace PlayerScripts
{
    public class FreeFall : PlayerState
    {
        protected internal FreeFall(Player player, PlayerStateMachine stateMachine)
            : base(player, stateMachine)
        {
            //if (Player.thruster.isPlaying) Player.thruster.Stop();
        }

        public override void Enter()
        {
            base.Enter();
            //if (BoosterSfxSource.clip != Player.soundData[2].clip) BoosterSfxSource.clip = Player.soundData[2].clip;
        }

        public override void Update()
        {
            base.Update();
            switch (Player.OnTheWall)
            {
                case true when Input.GetButtonDown("Jump"):
                    StateMachine.ChangeState(StateMachine.GetState<WallJump>());
                    break;
                case true:
                    StateMachine.ChangeState(StateMachine.GetState<WallSlide>());
                    break;
                default:
                    if (Player.IsGrounded)
                        StateMachine.ChangeState(StateMachine.GetState<PlayerGroundedState>());
                    break;
            }
        }

        public override void Exit()
        {
            base.Exit();
            //if (Player.thruster.isPlaying) Player.thruster.Stop();
            //if (BoosterSfxSource.isPlaying) BoosterSfxSource.Stop();
        }

        protected override void Locomotion()
        {
            base.Locomotion();

            if (Input.GetButton("Jump"))
            {
                Player.Rb.AddForce(Movement * Vector2.one, ForceMode2D.Force);
                //if (!Player.thruster.isPlaying) Player.thruster.Play();
                //if (!BoosterSfxSource.isPlaying) BoosterSfxSource.Play();
            }
            else
            {
                Player.Rb.AddForce(Player.accInAir * Movement * Vector2.right, ForceMode2D.Force);
                //if (Player.thruster.isPlaying) Player.thruster.Stop();
                //if (BoosterSfxSource.isPlaying) BoosterSfxSource.Stop();
            }
        }
    }
}
