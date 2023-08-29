using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Endsley
{
    public class MechController2 : MonoBehaviour, IMechController
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

        public MechLocomotionConfig LocomotionConfig => throw new NotImplementedException();
        #endregion

        #region Events
        public event Action<float> OnSpeedChange;
        public event Action<RotationDirection> OnRotationChange;
        public event Action OnJump;
        public event Action<bool> OnGroundedChange;
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MoveForward(float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveBackward(float speed)
        {
            throw new NotImplementedException();
        }

        public void StopMoving()
        {
            throw new NotImplementedException();
        }

        public void RotateContinuous(float direction)
        {
            throw new NotImplementedException();
        }

        public void RotateToHeading(float targetHeading)
        {
            throw new NotImplementedException();
        }

        public void Jump()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPosition()
        {
            throw new NotImplementedException();
        }

        public float GetCurrentSpeed()
        {
            throw new NotImplementedException();
        }

        public float GetCurrentHeading()
        {
            throw new NotImplementedException();
        }

        public bool IsGrounded()
        {
            throw new NotImplementedException();
        }
    }
}