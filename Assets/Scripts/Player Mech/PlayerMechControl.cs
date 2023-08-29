using UnityEngine;
using UnityEngine.InputSystem;

namespace Endsley
{
    public class PlayerMechControl : MonoBehaviour
    {
        private MechController mechController;
        [SerializeField] private InputAction moveControls;
        [SerializeField] private InputAction jumpControls;

        void Start()
        {
            // Try to find the MechController on this gameObject.
            if (!TryGetComponent(out mechController))
            {
                Debug.LogError("MechController not found on the GameObject. Please add one.");
                return;
            }

            // Check if InputActions are initialized
            if (moveControls == null || jumpControls == null)
            {
                Debug.LogError("InputActions for movement or jump are not set up. Please assign them.");
            }
        }


        void OnEnable()
        {
            moveControls.Enable();
            moveControls.performed += HandleMove;
            moveControls.canceled += HandleMove;

            jumpControls.Enable();
            jumpControls.performed += HandleJump;
        }

        void OnDisable()
        {
            moveControls.Disable();
            moveControls.performed -= HandleMove;
            moveControls.canceled -= HandleMove;

            jumpControls.Disable();
            jumpControls.performed -= HandleJump;
        }

        void HandleMove(InputAction.CallbackContext context)
        {
            Vector2 moveVector = context.ReadValue<Vector2>();
            mechController.UpdateControl(moveVector);
        }

        void HandleJump(InputAction.CallbackContext context)
        {
            mechController.Jump();
        }
    }
}
