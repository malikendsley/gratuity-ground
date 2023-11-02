using System.Collections;
using System.Collections.Generic;
using Endsley;
using UnityEngine;

// TODO: Extract base missile class
// TODO: Integrate with weapons bus
public class AIMissileLauncher : MonoBehaviour, IWeapon
{

    //TODO: put this in a config file
    #region Serialized Fields
    [Tooltip("The spread of this missile")]
    [SerializeField] private float spread = 5f;
    [Tooltip("How many missiles to fire per salvo")]
    [SerializeField] private int salvoSize;
    [SerializeField] private float launchDelay = 0.25f;
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
    private GameObject trackedTarget;
    private WeaponsBus weaponsBus;

    #endregion

    // Missiles do not reload
    public void Reload()
    {
        return;
    }

    // Set externally by the weapon manager
    public void SetTarget(GameObject target) => trackedTarget = target;

    // AI Missiles are semi-dumb fire
    // Capture the target on fire, fire at that point
    // If not perfect shot, capture a random point near the target and fire at that point
    public void StartWeapon(bool isPerfectShot)
    {
        if (trackedTarget)
        {
            // Fire at the frozen position
            StartCoroutine(FireMissilesCoroutine());
            return;
        }
        Debug.LogWarning("AI Fire with no target: " + gameObject.name);
    }

    // AI missiles have no locking behavior, no need for detecting holds
    public void StopWeapon()
    {
        return;
    }

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
        }
        else
        {
            Debug.LogWarning("No MechWeaponManager found in the parent hierarchy. This weapon will not be managed.");
        }
    }

    private IEnumerator FireMissilesCoroutine()
    {
        Debug.Log("Firing " + salvoSize + " missiles at " + trackedTarget.name + "last location ...");
        for (int i = 0; i < salvoSize; i++)
        {
            Transform currentFirePoint = firePoints[fpIndex];
            GameObject missile = Instantiate(missilePrefab, currentFirePoint.position, currentFirePoint.rotation);
            // Debug.Log("Missile being initialized to fire at " + lockTarget.name + " with allegiance " + bulletAllegiance + "...");
            missile.GetComponent<Missile>().Initialize(trackedTarget.transform.position + Random.insideUnitSphere * spread, Allegiance.Enemy);
            fpIndex = (fpIndex + 1) % firePoints.Count;

            yield return new WaitForSeconds(launchDelay);
        }
    }
}
