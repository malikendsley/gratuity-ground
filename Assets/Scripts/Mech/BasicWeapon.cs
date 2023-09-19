using System;
using UnityEngine;
using System.Collections;

namespace Endsley
{
    public class Basic1AxisWeapon : MonoBehaviour, IWeapon
    {
        #region Serialized Fields
        [SerializeField] private GameObject trackingTarget;
        [SerializeField] private GameObject target;
        [SerializeField] private Transform defaultForward;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Transform fireThrough;
        [SerializeField] private float fireRate;
        [SerializeField] private float magazineSize;
        [SerializeField] private float reloadTime;
        [SerializeField] private BulletAllegiance bulletAllegiance;
        //HACK: go back to pooling once it's fixed
        [SerializeField] private GameObject bulletPrefab;
        [Tooltip("Set this if you want to control the slot this weapon is in (for player weapons)")]
        // Add getter and setter for AssignedSlot with default value of -1
        // However, make them do nothing
        // (We are just enforcing the interface, not actually exposing the value)
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
        private bool aimAssistOn;
        private float fireDelay;
        private float nextShotTime = 0;
        private float nextReloadTime = 0;
        [SerializeField] float currentAmmo;
        private bool isReloading = false;
        #endregion

        #region Events
        public event Action<int> OnAmmoChange;
        public event Action OnWeaponStart;
        public event Action OnWeaponStop;
        public event Action OnReload;
        #endregion
        private void Start()
        {
            fireDelay = 1 / fireRate;
            currentAmmo = magazineSize; // Initialize the ammo

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
            // lookat target > trackingTarget > defaultForward
            LookAt(target != null ? target : trackingTarget != null ? trackingTarget : defaultForward.gameObject);

            //If we should fire, try to fire every time the time is greater than the next shot time
            if (shouldFire)
            {
                // If we are not reloading and the weapon is ready, fire
                if (Time.time > nextShotTime && !isReloading)
                {
                    if (currentAmmo > 0)
                    {
                        // Get bullet from pool and fire
                        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation).GetComponent<Bullet>();
                        if (!bullet)
                        {
                            Debug.LogWarning("No bullet available in pool. Consider increasing pool size.");
                            return;
                        }
                        if (aimAssistOn)
                        {
                            bullet.InitAimAssist(firePoint.position, bulletAllegiance, target.transform);
                        }
                        else
                        {
                            bullet.InitNoAimAssist(firePoint.position, fireThrough.position - firePoint.position, bulletAllegiance);
                        }

                        // Usual code
                        Debug.Log("Firing Weapon");
                        nextShotTime = Time.time + fireDelay;
                        currentAmmo--;
                        OnAmmoChange?.Invoke((int)currentAmmo);
                    }
                    else
                    {
                        Debug.Log("No Ammo");
                    }
                }
                else
                {
                    Debug.Log("Can't fire; Still reloading or waiting for next shot");
                }
            }
        }



        #region Weapon Actions
        public void StartWeapon(bool isPerfectShot)
        {
            OnWeaponStart?.Invoke();
            shouldFire = true;
            aimAssistOn = isPerfectShot;
        }

        public void StopWeapon()
        {
            OnWeaponStop?.Invoke();
            shouldFire = false;
            aimAssistOn = false;
        }


        public void Reload()
        {
            if (Time.time > nextReloadTime && !isReloading)
            {
                StartCoroutine(ReloadAfterDelay(reloadTime));
            }
            else
            {
                Debug.Log("Can't reload; Still reloading");
            }
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }
        #endregion

        #region Private Functions

        private IEnumerator ReloadAfterDelay(float delay)
        {
            isReloading = true;
            nextReloadTime = Time.time + delay;

            yield return new WaitForSeconds(delay);

            currentAmmo = magazineSize; // Fully reload
            OnReload?.Invoke(); // Notify subscribed components
            OnAmmoChange?.Invoke((int)currentAmmo); // Notify ammo change
            isReloading = false;
        }

        private void LookAt(GameObject target)
        {
            Vector3 ownVector = transform.InverseTransformPoint(defaultForward.position);
            Vector3 dirVector = transform.InverseTransformPoint(target.transform.position);
            Vector3 toTarget = target.transform.position - transform.position;
            Vector3 planeNormal = -UtilScripts.GetRootTransform(transform).right;
            float dotProduct = Vector3.Dot(planeNormal, toTarget);
            float angle = Vector3.Angle(ownVector, dirVector);
            angle *= dotProduct > 0 ? 1 : -1;
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
        }
        #endregion

    }
}