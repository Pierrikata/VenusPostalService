using UnityEngine;

namespace PlayerScripts
{
    public class Jump : PlayerState
    {
        protected internal Jump(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
        {
        }
    
        private float _buttonPressTime;
    
        public override void Enter()
        {
            base.Enter();

            Player.Rb.AddForce(Vector2.up * Player.jumpScale, ForceMode2D.Impulse);
            _buttonPressTime = 0;
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
                {
                    if (Player.Rb.linearVelocity.y < 0)
                        StateMachine.ChangeState(StateMachine.GetState<FreeFall>());
                    break;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _buttonPressTime += Time.deltaTime;
            if (_buttonPressTime < Player.buttonPressedWindow && Input.GetButton("Jump"))
                Player.Rb.AddForce(Vector2.up * (Player.jumpScale * Player.variableJumpAmp), ForceMode2D.Force);
        }

        protected override void Locomotion()
        {
            base.Locomotion();
            Player.Rb.AddForce(Player.accInAir * Movement * Vector2.right, ForceMode2D.Force);
        }
    }
}