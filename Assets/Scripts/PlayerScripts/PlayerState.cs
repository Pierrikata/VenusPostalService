using System;
using System.Collections.Generic;
using GeneralInterfaces;
using JetBrains.Annotations;
using StateMachineScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerState : BaseState, IActionCamera, IPivotObj2D
    {
        #region Fields
        protected readonly Player Player;
        protected readonly PlayerStateMachine StateMachine;
        protected Vector2 TargetVelocity, SpeedDif, AccelVector;
        protected static Vector2 MoveInput, Movement;
        protected float StateTimer;
        protected Vector3 Diff; // for flight vehicle embark
        protected GameObject GrabbedObject;
        protected List<GameObject> GrabbedObjects;
        
        private bool _canScoutAhead;
        private float _rotRadZ, _rotDegZ;
        private Vector3 _lookAheadPosition;
        private IActionCamera _actionCameraImplementation;
        protected Camera Camera;
        #endregion
        #region Properties

        protected PlayerState([NotNull] Player player, [NotNull] PlayerStateMachine stateMachine)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
            _canScoutAhead = false;
            StartCamera();
        }

        public override void Enter()
        {
            if (StateMachine.CurrentState is null) throw new NotImplementedException();
        }

        public override void Update()
        {
            MoveInput.x = Input.GetAxisRaw("Horizontal");
            MoveInput.y = Input.GetAxisRaw("Vertical");
            
            // TODO: if holding left shift and embarked on VTOL, enter flight state
            // TODO: set default conditions for scouting ahead
            
            if (Input.GetKeyDown(KeyCode.X) && Player.IsGrounded)
                _canScoutAhead = !_canScoutAhead; // Toggle Scout Ahead
        }

        public override void FixedUpdate()
        {
            StateTimer -= Time.deltaTime;
            FlipController();
            Locomotion();
            
            if(Input.GetKeyDown(KeyCode.F)) GrabObject();
            
            if (_canScoutAhead)
                ScoutAhead();
            else
            {
                DynamicZoom();
                if (!Player.framingTranspose.TargetOffset.Equals(Vector3.zero))
                    ResetTargetOffset();
            }
        }

        public override void Exit()
        {
            if (Player.transform.localRotation.z != 0)
                ResetRotation();
        }
        #endregion
        #region Methods
        
        protected virtual void FlipController()
        {
            if ((MoveInput.x > 0 && !Player.FacingRight) || (MoveInput.x < 0 && Player.FacingRight)) Player.Flip();
        }
        protected virtual void Locomotion()
        {
            // calculate the direction we want to move in and our desired velocity
            TargetVelocity = MoveInput.normalized * Player.maxSpeed;
            // calculate difference b/w current velocity and desired velocity
            SpeedDif = TargetVelocity - Player.Rb.linearVelocity;
            // acceleration rate adjusts accordingly
            AccelVector.x = (Mathf.Abs(TargetVelocity.x) > .01f) ? Player.runAcceleration : Player.runDeceleration;
            AccelVector.y = (Mathf.Abs(TargetVelocity.y) > .01f) ? Player.runAcceleration : Player.runDeceleration;
            
            /* applies acceleration to speed difference, then raises to a set power so acceleration increases
             with higher speeds */
            Movement.x = Mathf.Pow(Mathf.Abs(SpeedDif.x) * AccelVector.x,
                Player.velocityPower) * Mathf.Sign(SpeedDif.x);
            Movement.y = Mathf.Pow(Mathf.Abs(SpeedDif.y) * AccelVector.y,
                Player.velocityPower) * Mathf.Sign(SpeedDif.y);

            // applies force to rigid body, multiplying by Vector2.right so that it only affects X axis
            /*
             *  if (Player.IsGrounded)
                   Rb.AddForce(Movement * Vector2.right, ForceMode2D.Force);
                else
                    Rb.AddForce(Player.accInAir * Movement * Vector2.right, ForceMode2D.Force);
             */
        }
        protected void SetVelocity(Vector2 direction)
        {
            if (Player.Rb.linearVelocity.magnitude >= Player.flightSpeed &&
                (Player.Rb.linearVelocity.normalized != direction.normalized * Player.Rb.linearVelocity.magnitude))
                Player.Rb.AddForce(direction.normalized * Player.flightSpeed, ForceMode2D.Impulse);
            else
                Player.Rb.linearVelocity = direction.normalized * Player.flightSpeed;
        }
        private void ResetRotation()
        {
            if (Mathf.Abs(Player.transform.localRotation.z) < Mathf.Epsilon) return;
            var tempVelocity = Player.Rb.linearVelocity;
            
            Player.Rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation; // Temporarily unlock rotation constraints
            Player.transform.localRotation = Quaternion.Euler(Player.transform.localRotation.eulerAngles.x,
                Player.transform.localRotation.eulerAngles.y, 0); // Reset z-rotation
            Player.Rb.constraints |= RigidbodyConstraints2D.FreezeRotation; // Restore rotation constraints
            Player.Rb.linearVelocity = tempVelocity; // Restore velocity
        }
        private void ResetTargetOffset()
        {
            Player.CurrentTargetOffset = Vector3.Lerp(Player.CurrentTargetOffset, Vector3.zero,
                Time.deltaTime * Player.zoomOutSpeed * 2);
            Player.framingTranspose.TargetOffset = Player.CurrentTargetOffset;
        }
        
        #endregion
        #region Camera Implementation

        public void StartCamera()
        {
            Player.CurrentOrthoSize = Player.playerCam.Lens.OrthographicSize;
            Player.CurrentTargetOffset = Vector3.zero;
            Camera = Camera.main;
        }
        
        public virtual void DynamicZoom()
        {
            // Calculate the target orthographic size based on the magnitude of rigidbody velocity
            float targetOrthoSize =
                    Mathf.Lerp(Player.minOrthoSize, Player.maxOrthoSize, Player.Rb.linearVelocity.magnitude / Player.maxSpeed),
                zoomSpeed = Player.Rb.linearVelocity.magnitude > Player.CurrentOrthoSize
                    ? Player.zoomOutSpeed
                    : Player.zoomInSpeed;
            // Gradually adjust camera's orthographic size towards the target size
            Player.CurrentOrthoSize = Mathf.Lerp(Player.CurrentOrthoSize, targetOrthoSize, Time.deltaTime * zoomSpeed);
            Player.playerCam.Lens.OrthographicSize = Player.CurrentOrthoSize;
        }

        private void ScoutAhead() 
        {
            _lookAheadPosition = Player.transform.position
                                + (Camera.ScreenToWorldPoint(Input.mousePosition) - Player.transform.position).normalized
                                * Player.lookAheadDistance;
            Player.CurrentTargetOffset = Vector3.Lerp(
                Player.CurrentTargetOffset,
                new Vector3(_lookAheadPosition.x - Player.transform.position.x, _lookAheadPosition.y - Player.transform.position.y, 0),
                Time.deltaTime * Player.zoomOutSpeed * 1.2f
                );
            Player.framingTranspose.TargetOffset = Player.CurrentTargetOffset;
            Player.CurrentOrthoSize = Mathf.Lerp(Player.CurrentOrthoSize, Player.maxOrthoSize * 1.1f,
                Time.deltaTime * Player.zoomOutSpeed);
            Player.playerCam.Lens.OrthographicSize = Player.CurrentOrthoSize;
        }

        #endregion
        
        public void PivotRotate(Vector3 position) // IPivotObj2D implementation
        {
            Diff = (position - Player.transform.localPosition).normalized;
            _rotRadZ = Mathf.Atan2(Diff.y, Diff.x);
            _rotDegZ = _rotRadZ * Mathf.Rad2Deg;

            Player.transform.localRotation = _rotDegZ is < -90 or > 90
                ? Quaternion.Euler(180, 180, _rotDegZ)
                : Quaternion.Euler(0, 0, _rotDegZ);
        }

        void GrabObject()
        {
            if (!Player.ParcelInfo)
                return;
            var parcelBody = Player.ParcelInfo.gameObject.GetComponent<Rigidbody2D>();
            
            // grab
            if (!GrabbedObject)
            {
                GrabbedObject = Player.ParcelInfo.gameObject;
                parcelBody.bodyType = RigidbodyType2D.Kinematic;
                GrabbedObject.transform.position = Player.holdPoint.position;
                GrabbedObject.transform.SetParent(Player.holdPoint);
            }
            // release
            else
            {
                parcelBody.bodyType = RigidbodyType2D.Dynamic;
                parcelBody.linearVelocity = Player.Rb.linearVelocity;
                GrabbedObject.transform.SetParent(null);
                GrabbedObject = null;
            }
        }

        void LoadGrabbedObject()
        {
            if (GrabbedObject != null)
            {
                GrabbedObjects.Add(GrabbedObject);
                GrabbedObject.transform.SetParent(null);
                GrabbedObject = null;
            }
        }
    }
}
