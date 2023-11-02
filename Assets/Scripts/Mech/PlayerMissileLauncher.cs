using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For the purposes of this file, a "lock" is a single instance of a pending
// missile launch. The current target is based on where the reticle is pointing.
// When the button is held, the weapon will begin acquiring locks on the current
// target. When the button is released, the weapon will fire a missile at each
// lock. The weapon will continue to acquire locks until the button is released,
// up to the maximum number of locks the weapon can maintain.

// NOTE: If the firing points look janky, look into resetting fpIndex to 0

namespace Endsley
{
    public class PlayerMissileLauncher : MonoBehaviour, IWeapon
    {
        //TODO: put this in a config file
        #region Serialized Fields
        [Tooltip("The maximum number of locks the weapon can maintain")]
        [SerializeField] private int maxLocks = 4;
        [Tooltip("Seconds per lock acquisition")]
        [SerializeField] private float lockTime = 1f;
        [Tooltip("Delay per missile launch")]
        [SerializeField] private float launchDelay = 0.25f;
        [Tooltip("Minimum delay (This is added on top of the per missile launch delay)")]
        [SerializeField] private float minimumDelay = 0.5f;
        [Tooltip("Missile prefab")]
        [SerializeField] private GameObject missilePrefab;
        [Tooltip("Where the missile initially spawns")]
        [SerializeField] private List<Transform> firePoints;
        private int fpIndex = 0;

        [SerializeField] private int assignedSlot = -1;
        public int AssignedSlot
        {
            // Make the getter print a warning and return -1
            get
            {
                Debug.LogWarning("This interface is only to enforce the interface. You shouldn't need this value.");
                return assignedSlot;
            }
            set { }
        }
        #endregion

        #region Private Variables
        private bool shouldFire;
        private GameObject trackedTarget;
        private GameObject lastTrackedTarget;
        private float nextFireTime = 0;
        private readonly List<GameObject> locks = new();
        private WeaponsBus weaponsBus;
        #endregion

        private void Start()
        {
            // Get the weapon bus for this mech
            weaponsBus = WeaponsBusManager.Instance.GetOrCreateBus(GetComponentInParent<MechController>().gameObject);
            if (weaponsBus == null)
            {
                Debug.LogError("No WeaponsBus found. This weapon will not be managed.");
            }
            // Register self with Weapon Manager
            MechWeaponManager weaponManager = GetComponentInParent<MechWeaponManager>();
            if (weaponManager)
            {
                if (assignedSlot == -1)
                {
                    _ = weaponManager.RegisterWeaponNext(this);
                }
                else
                {
                    weaponManager.RegisterWeapon(this, assignedSlot);
                }
                // Do something with assignedSlot if needed
            }
            else
            {
                Debug.LogWarning("No MechWeaponManager found in the parent hierarchy. This weapon will not be managed.");
            }
        }

        private void Update()
        {
            // Every locktime seconds, acquire a lock on the current target if 
            // we are still locked onto the same target switching targets resets
            // the lock timer, but does not reset the
            // lock list
            if (shouldFire)
            {
                if (trackedTarget != lastTrackedTarget)
                {
                    lastTrackedTarget = trackedTarget;
                    nextFireTime = Time.time + lockTime;  // Reset the lock timer only if target changes
                }
                if (Time.time > nextFireTime)
                {
                    // In here we have passed the first check so this will work
                    if (locks.Count < maxLocks && trackedTarget != null)
                    {
                        locks.Add(trackedTarget);
                        nextFireTime = Time.time + lockTime;
                        weaponsBus.Emit(WeaponEventType.OnLockStack, new WeaponEventData { Target = trackedTarget, Weapon = this });
                        Debug.Log("Stacking lock on " + trackedTarget.name);
                    }
                    else
                    {
                        nextFireTime = Time.time + lockTime;
                    }
                }
            }
        }

        public void Reload()
        {
            return;
        }

        public void SetTarget(GameObject target)
        {
            trackedTarget = target;
        }

        // For a missile launcher, begins acquiring locks on the current target
        // Ignore isPerfectShot, as missile launchers do not use this
        public void StartWeapon(bool isPerfectShot)
        {
            // Begin acquiring locks
            Debug.Log("Locking...");
            shouldFire = true;
            weaponsBus.Emit(WeaponEventType.OnWeaponStart, new WeaponEventData { Target = trackedTarget, Weapon = this });
        }

        // Fire a missile at each lock, if any and reset the lock list
        public void StopWeapon()
        {
            shouldFire = false;
            if (locks.Count > 0)
            {
                Debug.Log("Locks found. Firing...");
                FireMissiles(locks);
                locks.Clear();
            }
            else
            {
                Debug.Log("No locks found. Not firing.");
            }
            weaponsBus.Emit(WeaponEventType.OnWeaponStop, new WeaponEventData { Target = trackedTarget, Weapon = this });
        }

        private void FireMissiles(List<GameObject> locks)
        {
            Debug.Log("Firing Weapon");
            // Lock out the missile launcher for the time it takes to fire all missiles
            nextFireTime = Time.time + launchDelay * locks.Count + minimumDelay;
            StartCoroutine(FireMissilesCoroutine(locks));
        }

        private IEnumerator FireMissilesCoroutine(List<GameObject> locks)
        {
            // Debug.Log("Firing " + locks.Count + " missiles");
            // The locks variable will be modified by the coroutine, so we need to
            // make a copy of it
            List<GameObject> locksCopy = new(locks);
            foreach (GameObject lockTarget in locksCopy)
            {
                Transform currentFirePoint = firePoints[fpIndex];
                GameObject missile = Instantiate(missilePrefab, currentFirePoint.position, currentFirePoint.rotation);
                // Debug.Log("Missile being initialized to fire at " + lockTarget.name + " with allegiance " + bulletAllegiance + "...");
                missile.GetComponent<Missile>().Initialize(lockTarget, Allegiance.Player);
                // Cycle the fire point index
                fpIndex = (fpIndex + 1) % firePoints.Count;

                yield return new WaitForSeconds(launchDelay);
            }
        }
    }
}