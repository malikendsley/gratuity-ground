using System;
using UnityEngine;
using System.Collections;
using Endsley;

public class Basic1AxisWeapon : MonoBehaviour, IWeapon
{
    #region Serialized Fields
    [SerializeField] private GameObject trackedTarget;
    [SerializeField] private Transform defaultForward;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate;
    [SerializeField] private float magazineSize;
    [SerializeField] private float reloadTime;
    #endregion

    #region Private Variables
    private float fireDelay;
    private float nextShotTime;
    private float nextReloadTime;
    [SerializeField] float currentAmmo;
    private Quaternion defaultRot;
    private bool isReloading = false;
    #endregion

    #region Events
    public event Action<int> OnAmmoChange;
    public event Action OnWeaponPrep;
    public event Action<bool> OnWeaponFire;
    public event Action OnReload;
    #endregion
    private void Start()
    {
        fireDelay = 1 / fireRate;
        currentAmmo = magazineSize; // Initialize the ammo
        defaultRot = transform.rotation;

        // Register self with Weapon Manager
        MechWeaponManager weaponManager = GetComponentInParent<MechWeaponManager>();
        if (weaponManager)
        {
            //NOTE: If needed, can retrieve what slot you're in from here.
            _ = weaponManager.RegisterWeaponNext(this);
            // Do something with assignedSlot if needed
        }
        else
        {
            Debug.LogWarning("No MechWeaponManager found in the parent hierarchy. This weapon will not be managed.");
        }
    }
    private void Update()
    {
        if (trackedTarget)
        {
            LookAt(trackedTarget);
        }
        else
        {
            transform.rotation = defaultRot;
        }
    }
    #region Weapon Actions
    public void FireWeapon()
    {
        bool success = false;
        if (Time.time > nextShotTime && !isReloading)
        {
            if (currentAmmo > 0)
            {
                Debug.Log("Firing Weapon");
                nextShotTime = Time.time + fireDelay;
                currentAmmo--;
                OnAmmoChange?.Invoke((int)currentAmmo);
                success = true;
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

        OnWeaponFire?.Invoke(success);

    }

    public void PrepWeapon()
    {
        OnWeaponPrep?.Invoke();
        Debug.LogWarning("No prep behavior");
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

    public void StopWeapon()
    {
        Debug.LogWarning("No cancel behavior");
    }

    public void SetTarget(GameObject target) => trackedTarget = target;
    #endregion

    #region Private Functions

    private IEnumerator ReloadAfterDelay(float delay)
    {
        isReloading = true;
        nextReloadTime = Time.time + delay;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(delay);

        currentAmmo = magazineSize; // Fully reload
        OnReload?.Invoke(); // Notify subscribed components
        OnAmmoChange?.Invoke((int)currentAmmo); // Notify ammo change
        isReloading = false;
        Debug.Log("Reload Complete");
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
