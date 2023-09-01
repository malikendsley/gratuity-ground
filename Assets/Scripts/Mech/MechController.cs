using System;
using UnityEngine;

namespace Endsley
{
    public class MechController : MonoBehaviour, IMechController
    {

        #region Serialized Fields
        [Header("Components")]
        [SerializeField] private MechLocomotionConfig locomotionConfig;
        public MechLocomotionConfig LocomotionConfig => locomotionConfig;

        [SerializeField] private CharacterController characterController;

        [Header("Grounding")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float checkDistance;
        [SerializeField] private LayerMask checkMask;
        [SerializeField] private bool usesHeading = false;
        #endregion

        #region Input Variables
        private float targetSpeed = 0f;
        private float currentSpeed = 0f;
        private bool tryJump = false;
        #endregion

        #region State Variables
        private float verticalVelocity;
        private const float Gravity = 9.81f;
        private float rotationDelta = 0f;
        private float lastRotationDelta = 0f;
        private float desiredHeading;


        private bool wasGrounded;
        private bool isGrounded;

        #endregion

        #region Events
        public event Action<float> OnSpeedChange;
        public event Action<float> OnRotationChange;
        public event Action OnJump;
        public event Action<bool> OnGroundedChange;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            wasGrounded = isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);
        }

        // Update is called once per frame
        void Update()
        {
            // Controlled by Jump()
            HandleGrounding();
            // Controlled by UpdateControl()
            HandleRot();
            HandleMovement();
            HandleRotationTowardsDesiredHeading();

        }

        private void HandleGrounding()
        {
            wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);
            if (wasGrounded != isGrounded)
            {
                OnGroundedChange?.Invoke(isGrounded);
            }
            if (isGrounded)
            {
                if (tryJump) //Should jump
                {
                    verticalVelocity = locomotionConfig.jumpForce;
                    OnJump?.Invoke();
                }
            }
            else
            {
                verticalVelocity -= Gravity * Time.deltaTime;
            }
            // Consume the jump request anyway providing an "ignore" effect
            tryJump = false;
        }

        private void HandleRot()
        {
            transform.Rotate(0, rotationDelta, 0);

            if (rotationDelta != lastRotationDelta)
            {
                OnRotationChange?.Invoke(rotationDelta);
                lastRotationDelta = rotationDelta;
            }
        }

        private void HandleMovement()
        {
            float acceleration = (targetSpeed == 0f) ? locomotionConfig.deceleration : locomotionConfig.acceleration;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            OnSpeedChange?.Invoke(currentSpeed);
            // Add in jump motion and forward motion
            Vector3 moveVector = new(0, verticalVelocity * Time.deltaTime, 0);
            moveVector += currentSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
            characterController.Move(moveVector);
        }

        private void HandleRotationTowardsDesiredHeading()
        {
            if (usesHeading)
            {
                float currentHeading = transform.eulerAngles.y;
                float newHeading = Mathf.MoveTowardsAngle(currentHeading, desiredHeading, locomotionConfig.rotationSpeed * Time.deltaTime);
                transform.eulerAngles = new Vector3(0, newHeading, 0);

                float rotationDelta = newHeading - currentHeading;
                if (rotationDelta != lastRotationDelta)
                {
                    OnRotationChange?.Invoke(rotationDelta);
                    lastRotationDelta = rotationDelta;
                }
            }
        }

        public void UpdateControl(Vector2 ctrlVector)
        {
            //Extract forward / backward desired motion
            //TODO: If on subsequent mech models, controls are flipped, undo "-"
            targetSpeed = locomotionConfig.walkSpeed * -Math.Sign(ctrlVector.y);
            //Extract rotation desire
            rotationDelta = ctrlVector.x * locomotionConfig.rotationSpeed * Time.deltaTime;
        }

        public void StopMoving()
        {
            UpdateControl(new(0, 0));
            tryJump = false;
        }

        public void RotateToHeading(float targetHeading)
        {
            //convert to 360 degrees
            targetHeading = (targetHeading + 360) % 360;
            usesHeading = true;
            desiredHeading = targetHeading;
        }

        public void RotateToTarget(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0;  // Ensure the rotation is only around the Y-axis

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            float targetHeading = targetRotation.eulerAngles.y + 180;  // Add 180 degrees to face away

            RotateToHeading(targetHeading);
        }

        public void Jump()
        {
            tryJump = true;
        }

        public Vector3 Position => transform.position;
        public float CurrentSpeed => currentSpeed;
        public bool IsGrounded => isGrounded;
    }
}