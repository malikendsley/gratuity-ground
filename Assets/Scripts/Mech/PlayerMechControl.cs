using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Endsley
{
    public class PlayerMechControl : MonoBehaviour
    {
        // Singleton instance
        public static PlayerMechControl Instance { get; private set; }

        // Expose player's location
        public Transform PlayerTransform => transform;

        private MechController mechController;
        private MechWeaponManager mechWeaponManager;

        [SerializeField] private InputAction moveControls;
        [SerializeField] private InputAction jumpControls;
        [SerializeField] private InputAction fire1Control;
        [SerializeField] private InputAction fire2Control;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Your existing Start logic
            if (!TryGetComponent(out mechController))
            {
                Debug.LogError("MechController not found on the GameObject. Please add one.");
                return;
            }

            if (!TryGetComponent(out mechWeaponManager))
            {
                Debug.LogError("MechWeaponManager not found on the GameObject. Please add one.");
                return;
            }

            if (moveControls == null || jumpControls == null || fire1Control == null || fire2Control == null)
            {
                Debug.LogError("InputActions for movement, jump, or firing are not set up. Please assign them.");
            }

            mechWeaponManager.SetTargetForAll(ReticleWorldPosition.Instance.GetGameObject());
        }

        void OnEnable()
        {
            // Your existing OnEnable logic
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
            // Your existing OnDisable logic
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
            // Your existing logic
            Vector2 moveVector = context.ReadValue<Vector2>();
            mechController.UpdateControl(moveVector);
        }

        void HandleJump(InputAction.CallbackContext context)
        {
            // Your existing logic
            mechController.Jump();
        }

        void HandleFire1Down(InputAction.CallbackContext context)
        {
            // Your existing logic
            Debug.Log("Fire 1 pressed");
            mechWeaponManager.StartWeapon(1);
        }

        void HandleFire2Down(InputAction.CallbackContext context)
        {
            // Your existing logic
            Debug.Log("Fire 2 pressed");
            mechWeaponManager.StartWeapon(2);
        }

        void HandleFire1Up(InputAction.CallbackContext context)
        {
            // Your existing logic
            Debug.Log("Fire 1 released");
            mechWeaponManager.StopWeapon(1);
        }

        void HandleFire2Up(InputAction.CallbackContext context)
        {
            // Your existing logic
            Debug.Log("Fire 2 released");
            mechWeaponManager.StopWeapon(2);
        }

        public Vector3 GetNearbyPoint(float minRadius, float maxRadius)
        {
            float randomRadius = Random.Range(minRadius, maxRadius);
            Vector3 randomDirection = Random.insideUnitSphere * randomRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            Debug.LogWarning("Could not find a nearby point for GetNearbyPoint().");
            return Vector3.zero;
        }

    }
}
