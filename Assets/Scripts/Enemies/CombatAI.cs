using Endsley;
using UnityEngine;

// Simple independent AI that automatically attacks a set or acquired target based on distance.
public class CombatAI : MonoBehaviour
{
    //TODO: Convert this into a config like an "attack profile"
    public LayerMask playerMask;
    public float noticeRange = 100f;
    public float targetingRangeFar = 50f;
    public float targetingRangeNear = 10f;

    private bool forcedTargeting = false;
    private GameObject target;
    private MechWeaponManager mechWeaponManager;
    private AttackState state;
    private enum AttackState
    {
        Idle,
        TargetingFar,
        TargetingNear,
    }
    private void OnDrawGizmosSelected()
    {
        //Draw wire spheres for the various ranges
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, noticeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetingRangeFar);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, targetingRangeNear);
    }

    private void Start()
    {
        if (!TryGetComponent(out mechWeaponManager))
        {
            Debug.LogWarning("No MechWeaponManager found on this object. AI will not be able to attack.");
        }
    }

    private void Update()
    {
        // If targeting is forced, then just skip to handling the target
        // Otherwise, check for a target (at the moment, just the player)
        if (!forcedTargeting)
        {
            // A forced target overrides our normal targeting
            HandleTargeting();
        }
        HandleAttacking();
    }

    // These are run in a loop in Update()
    private void HandleTargeting()
    {
        // Get distance to player and see what range we're in
        target = CheckForTarget();

        //
    }

    private void HandleAttacking()
    {

    }

    private GameObject CheckForTarget()
    {
        // Cast a sphere to see if we're in range of any player objects
        // Return the appropriate Attack state if we are, otherwise return Idle
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, noticeRange, colliders, playerMask);
        if (colliders[0] != null)
        {
            // We found a player, check the distance
            float distance = Vector3.Distance(transform.position, colliders[0].transform.position);
            if (distance <= targetingRangeNear)
            {
                Debug.Log("Targeting near");
                state = AttackState.TargetingNear;
                return colliders[0].gameObject;
            }
            else if (distance <= targetingRangeFar)
            {
                Debug.Log("Targeting far");
                state = AttackState.TargetingFar;
                return colliders[0].gameObject;
            }
        }

        Debug.Log("Idle");
        state = AttackState.Idle;
        return null;
    }

    public void OverrideTargeting(GameObject target)
    {
        forcedTargeting = true;
        this.target = target;
    }

    public void ClearTargeting()
    {
        forcedTargeting = false;
        target = null;
    }
}
