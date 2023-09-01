using UnityEngine;
using UnityEngine.InputSystem;

namespace Endsley
{
    public class PlayerMechControl : MonoBehaviour
    {
        private MechController mechController;
        private MechWeaponManager mechWeaponManager;
        [SerializeField] private InputAction moveControls;
        [SerializeField] private InputAction jumpControls;
        [SerializeField] private InputAction fire1Control;
        [SerializeField] private InputAction fire2Control;

        void Start()
        {
            // Try to find the MechController on this gameObject.
            if (!TryGetComponent(out mechController))
            {
                Debug.LogError("MechController not found on the GameObject. Please add one.");
                return;
            }

            // Try to find the MechWeaponManager on this gameObject.
            if (!TryGetComponent(out mechWeaponManager))
            {
                Debug.LogError("MechWeaponManager not found on the GameObject. Please add one.");
                return;
            }

            // Check if InputActions are initialized
            if (moveControls == null || jumpControls == null || fire1Control == null || fire2Control == null)
            {
                Debug.LogError("InputActions for movement, jump, or firing are not set up. Please assign them.");
            }

            mechWeaponManager.SetTargetForAll(ReticleWorldPosition.Instance.GetGameObject());
        }


        void OnEnable()
        {
            moveControls.Enable();
            moveControls.performed += HandleMove;
            moveControls.canceled += HandleMove;

            jumpControls.Enable();
            jumpControls.performed += HandleJump;
            jumpControls.canceled += HandleJump;

            fire1Control.Enable();
            fire1Control.started += HandleFire1Down;
            fire1Control.canceled += HandleFire1Up;

            fire2Control.Enable();
            fire2Control.started += HandleFire2Down;
            fire2Control.canceled += HandleFire2Up;
        }
        void OnDisable()
        {
            moveControls.Disable();
            moveControls.performed -= HandleMove;
            moveControls.canceled -= HandleMove;

            jumpControls.Disable();
            jumpControls.performed -= HandleJump;
            jumpControls.canceled -= HandleJump;

            fire1Control.Disable();
            fire1Control.started -= HandleFire1Down;
            fire1Control.canceled -= HandleFire1Up;

            fire2Control.Disable();
            fire2Control.started -= HandleFire2Down;
            fire2Control.canceled -= HandleFire2Up;
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

        void HandleFire1Down(InputAction.CallbackContext context)
        {
            Debug.Log("Fire 1 pressed");
            mechWeaponManager.FireWeapon(1);
        }

        void HandleFire2Down(InputAction.CallbackContext context)
        {
            Debug.Log("Fire 2 pressed");
            mechWeaponManager.FireWeapon(2);
        }

        void HandleFire1Up(InputAction.CallbackContext context)
        {
            Debug.Log("Fire 1 released");
            mechWeaponManager.StopWeapon(1);
        }

        void HandleFire2Up(InputAction.CallbackContext context)
        {
            Debug.Log("Fire 2 released");
            mechWeaponManager.StopWeapon(2);
        }
    }
}
