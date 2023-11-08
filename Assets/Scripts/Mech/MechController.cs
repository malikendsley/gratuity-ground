using System;
using UnityEngine;

namespace Endsley
{
    public class MechController : MonoBehaviour, IMechController
    {

        #region Serialized Fields
        [Header("Components")]
        [SerializeField] private bool usePerspectiveControl = true;
        [SerializeField] private MechLocomotionConfig locomotionConfig;
        public MechLocomotionConfig LocomotionConfig => locomotionConfig;

        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera mechCamera;

        [Header("Grounding")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float checkDistance;
        [SerializeField] private LayerMask checkMask;

        [Header("Tuning")]
        [Tooltip("How fast the mech rotates to face the direction of movement")]
        [SerializeField] private float rotSpeed;
        #endregion

        #region Input Variables
        private Vector3 controlVector;
        private bool tryJump = false;
        #endregion

        #region State Variables
        private float verticalVelocity;
        private const float Gravity = 9.81f;
        private float speed;
        private bool isSprinting;


        private bool wasGrounded;
        private bool isGrounded;

        #endregion

        #region Events
        public event Action<float> OnSpeedChange;
        public event Action OnJump;
        public event Action<bool> OnGroundedChange;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            wasGrounded = isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);
            if (!mechCamera)
            {
                mechCamera = Camera.main;
                Debug.LogWarning("No camera assigned to MechController. Using main camera.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Controlled by Jump()
            HandleGrounding();
            // Controlled by UpdateControl()
            HandleMovement();
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

        private void HandleMovement()
        {
            float targetSpeed;
            if (controlVector == Vector3.zero)
            {
                targetSpeed = 0;
            }
            else
            {
                targetSpeed = isSprinting ? locomotionConfig.runSpeed : locomotionConfig.walkSpeed;
            }
            // Move speed towards target speed at acceleration rate
            speed = Mathf.MoveTowards(speed, targetSpeed, locomotionConfig.acceleration * Time.deltaTime);
            OnSpeedChange?.Invoke(speed);
            // Add in jump motion
            Vector3 moveVector = new(0, verticalVelocity * Time.deltaTime, 0);
            // Convert the 2 axis control vector to be relative to the camera and scale by speed
            // if usePerspectiveControl is true
            var adjustedControlVector = controlVector;
            if (usePerspectiveControl)
            {
                adjustedControlVector = mechCamera.transform.TransformDirection(controlVector);
            }
            moveVector += speed * Time.deltaTime * adjustedControlVector;
            characterController.Move(moveVector);
            // Rotate the mech to face the direction of movement scaled by rotSpeed
            // (The mech model is backwards, so we need to rotate 180 degrees)
            // Don't pitch the mech, only rotate on the Y axis
            if (adjustedControlVector != Vector3.zero)
            {
                // Extract horizontal direction by projecting the control vector onto the horizontal plane
                Vector3 horizontalDirection = Vector3.ProjectOnPlane(adjustedControlVector, Vector3.up);

                // Create a rotation that looks in the direction of horizontal movement
                Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
                targetRotation *= Quaternion.Euler(0, 180, 0); // Compensate for the mech model being backwards

                // Smoothly rotate towards the target rotation only on the Y axis
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotSpeed * Time.deltaTime);
            }
        }

        // Has the nice effect of natively supporting a stick
        public void UpdateControl(Vector2 ctrlVector)
        {
            // Clamp the magnitude to 1
            ctrlVector = Vector2.ClampMagnitude(ctrlVector, 1);

            // Store the vector for use in Update()
            controlVector = new Vector3(ctrlVector.x, 0, ctrlVector.y);
        }
        public void StopMoving()
        {
            UpdateControl(new(0, 0));
            tryJump = false;
        }

        public void SetSprint(bool sprinting)
        {
            Debug.Log("Setting sprint to " + sprinting);
            isSprinting = sprinting;
        }

        public void Jump() => tryJump = true;

        public Vector3 Position => transform.position;
        public float CurrentSpeed => speed;
        public bool IsGrounded => isGrounded;
    }
}