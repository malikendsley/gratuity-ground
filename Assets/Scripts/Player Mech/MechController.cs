using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Endsley
{
    public class MechController : MonoBehaviour
    {

        #region Serialized Fields
        [Header("Components")]
        [SerializeField] private MechLocomotionConfig locomotionConfig;
        [SerializeField] private CharacterController characterController;

        [Header("Grounding")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float checkDistance;
        [SerializeField] private LayerMask checkMask;

        [Header("Input")]
        [SerializeField] private InputAction moveControls;
        [SerializeField] private InputAction jumpControls;
        #endregion

        #region Private Variables
        private float targetSpeed = 0f;
        private float currentSpeed = 0f;
        private Vector2 moveVector = Vector2.zero;

        private float verticalVelocity;
        private const float Gravity = 9.81f;
        private bool isGrounded;
        #endregion

        #region Events
        public event Action<float> OnSpeedChangeAction;
        public event Action<RotationDirection> OnRotatingAction;
        public event Action OnJumpAction;
        public event Action<bool> OnGroundStateChangeAction;
        #endregion

        #region Enums
        public enum RotationDirection { CW, CCW }
        #endregion

        private void OnEnable()
        {
            moveControls.Enable();
            jumpControls.Enable();
        }

        private void OnDisable()
        {
            moveControls.Disable();
            jumpControls.Disable();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawSphere(groundCheck.position, checkDistance);
        }


        private void Update()
        {

            // TODO: Refactor this and separate input from mech movement
            // Gather input
            moveVector = moveControls.ReadValue<Vector2>();
            Debug.Log("Jump input was " + (jumpControls.triggered ? "" : "not ") + "triggered");

            // Delta type calculations
            bool wasGrounded = isGrounded;
            isGrounded = Physics.CheckSphere(groundCheck.position, checkDistance, checkMask);
            Debug.Log("Character is " + (isGrounded ? "" : "not ") + "grounded");

            if (wasGrounded != isGrounded)
            {
                //NOTE: This may be noisy, provide a grace period? Alternatively, harden subscribers against noise (seems like more work)
                OnGroundStateChangeAction?.Invoke(isGrounded);
            }

            // Handle jumping

            if (isGrounded)
            {
                if (jumpControls.triggered)
                {
                    verticalVelocity = locomotionConfig.jumpForce;
                    OnJumpAction?.Invoke();
                }
            }
            else
            {
                verticalVelocity -= Gravity * Time.deltaTime;
            }

            //TODO: Y needs to be flipped because for some reason the animations are backwards on the mech due to the way the bones were handled
            moveVector.y = -moveVector.y;

            //NOTE: This is more player movement, will need to be pulled out
            UpdateTargetSpeed(moveVector);
            float acceleration = targetSpeed == 0f ? locomotionConfig.deceleration : locomotionConfig.acceleration;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
            OnSpeedChangeAction?.Invoke(currentSpeed);
            //Debug.Log("Calling Move with: " + movement);
            Vector3 movement = new Vector3(0, verticalVelocity * Time.deltaTime, 0) + currentSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
            characterController.Move(movement);

            Rotate(moveVector);
        }

        public void UpdateTargetSpeed(Vector2 movementVector)
        {
            if (movementVector.y > 0)
            {
                targetSpeed = locomotionConfig.walkSpeed;
            }
            else if (movementVector.y < 0)
            {
                targetSpeed = -locomotionConfig.walkSpeed;
            }
            else
            {
                targetSpeed = 0;
            }
        }

        public void Rotate(Vector2 movementVector)
        {
            float rotationAmount = movementVector.x * locomotionConfig.rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
            if (rotationAmount != 0)
            {
                //TODO: May need to flip
                OnRotatingAction.Invoke(rotationAmount > 0 ? RotationDirection.CW : RotationDirection.CCW);
            }
        }

        public MechLocomotionConfig GetMechLocomotionConfig()
        {
            return locomotionConfig;
        }

    }
}