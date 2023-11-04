using System;
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
        [SerializeField] private InputAction reloadControl;

        private Action<WeaponEventData> HandleOnTargetChange;
        private Action<WeaponEventData> HandleOnWeaponClear;
        private WeaponsBus weaponsBus;
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

            if (moveControls == null || jumpControls == null || fire1Control == null || fire2Control == null || reloadControl == null)
            {
                Debug.LogError("Unassigned InputAction in PlayerMechControl. Please assign all InputActions.");
            }
            // TODO: remove this, the target is the destination for most weapons it needs to be set to an actual target
            HandleOnTargetChange = (WeaponEventData data) => mechWeaponManager.SetTargetForAll(data.Target);
            HandleOnWeaponClear = (WeaponEventData data) => mechWeaponManager.SetTargetForAll(null);
            weaponsBus = WeaponsBusManager.Instance.GetOrCreateBus(PlayerMechTag.Instance.PlayerMech);
            if (weaponsBus == null)
            {
                Debug.LogError("WeaponsBus not found for player mech. Please add one.");
            }
            else
            {
                weaponsBus.Subscribe(WeaponEventType.OnTargetChange, HandleOnTargetChange);
                weaponsBus.Subscribe(WeaponEventType.OnTargetClear, HandleOnWeaponClear);
            }
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

            reloadControl.Enable();
            reloadControl.started += HandleReload;

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

            reloadControl.Disable();
            reloadControl.started -= HandleReload;

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
            mechWeaponManager.StartWeapon(2);
        }

        void HandleFire1Up(InputAction.CallbackContext context)
        {
            // Your existing logic
            mechWeaponManager.StopWeapon(1);
        }

        void HandleFire2Up(InputAction.CallbackContext context)
        {
            // Your existing logic
            mechWeaponManager.StopWeapon(2);
        }

        void HandleReload(InputAction.CallbackContext context)
        {
            // Your existing logic
            mechWeaponManager.ReloadAllWeapons();
        }


        public Vector3 GetNearbyPoint(float minRadius, float maxRadius)
        {
            float randomRadius = UnityEngine.Random.Range(minRadius, maxRadius);
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * randomRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, randomRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
            Debug.LogWarning("Could not find a nearby point for GetNearbyPoint().");
            return Vector3.zero;
        }


        private void OnDestroy()
        {
            weaponsBus.Unsubscribe(WeaponEventType.OnTargetChange, HandleOnTargetChange);
        }

    }
}
