using System;
using StateMachineScripts;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerScripts
{
    public class Player : MonoBehaviour
    {
        #region Fields
        
        [Header("CineMachine")] [SerializeField] internal CinemachineCamera playerCam;
        [SerializeField] internal CinemachinePositionComposer framingTranspose;
        [SerializeField] internal float zoomOutSpeed = .75f, zoomInSpeed = .25f,
            minOrthoSize = 8, maxOrthoSize = 30, lookAheadDistance = 2;
        protected internal float CurrentOrthoSize;
        protected internal Vector3 CurrentTargetOffset;
        
        [Header("Movement")]
        [SerializeField] internal float maxSpeed = 22,
            jumpScale,
            variableJumpAmp,
            wallJumpAmp,
            buttonPressedWindow,
            runAcceleration = 3,
            runDeceleration = 4,
            accInAir = .15f,
            velocityPower,
            flightSpeed;
        
        [Header("Collisions")] [SerializeField] private LayerMask whatIsGround;
        [SerializeField] internal Transform groundCheck, wallCheck;
        [SerializeField] private float groundCheckRadius, wallCheckRadius;
        
        #endregion
        #region Components
        
        //public Animator Anim { get; private set; }
        public Rigidbody2D Rb { get; private set; }
        private readonly PlayerStateMachine _stateMachine;
        
        #endregion
        
        public Player() { _stateMachine = new PlayerStateMachine(this); }

        #region Core Methods

        private void Awake() { _stateMachine.Awake(); }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Rb = GetComponent<Rigidbody2D>();
            _stateMachine.Initializer(_stateMachine.GetState<PlayerGroundedState>());
        }

        // Update is called once per frame
        void Update() { _stateMachine.Update(); }
        private void FixedUpdate() { _stateMachine.FixedUpdate(); }
        
        #endregion
        public BaseState GetCurrentState() { return _stateMachine.CurrentState; }
        
        #region Flip

        public int FacingDir { get; private set; } = 1;
        public bool FacingRight { get; private set; } = true;
        public void Flip()
        {
            FacingDir *= -1;
            SetBoolFacingRight();
            // transform.Rotate(0, 180, 0);
        
            //stores scale and flips the player along the x-axis, 
            var scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        private void SetBoolFacingRight() { FacingRight = !FacingRight; }

        #endregion
        #region Collisions

        public bool IsGrounded => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        public bool OnTheWall => Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsGround);
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        #endregion
    }
}
