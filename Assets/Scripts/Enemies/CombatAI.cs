using Endsley;
using UnityEngine;
using System;
using System.Collections;
// Simple independent AI that automatically attacks a set or acquired target based on distance.
public class CombatAI : MonoBehaviour
{
    //TODO: Convert this into a config like an "attack profile"
    public LayerMask playerMask;
    [Tooltip("Layers to ignore when checking for line of sight")]
    public LayerMask ignoreMask;
    public float targetingRangeFar = 50f;
    //TODO: Difficulty scaling
    [Tooltip("Seconds between attacks")]
    public float attackRateFar = 6f;
    public float targetingRangeNear = 10f;
    [Tooltip("Seconds between attacks")]
    public float attackRateNear = 3f;
    [Tooltip("Seconds to fire for")]
    public float salvoDuration = 2f;
    [Tooltip("Accuracy of the AI's aim. Percentage that a salvo will fire with perfect accuracy.")]
    [Range(0f, 1f)]
    public float accuracy = 0.5f;
    private GameObject target;
    private MechWeaponManager mechWeaponManager;
    private AttackState state;
    private float lastAttackTime = 0f;
    private float currentAttackRate;
    private bool isFiring = false;
    private float salvoEndTime = 0f;


    private enum AttackState
    {
        Idle,
        TargetingFar,
        TargetingNear,
    }
    private void OnDrawGizmosSelected()
    {
        //Draw wire spheres for the various ranges
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
        // Check for a playermechcontrol on this mech and raise an error if so
        // (We cannot have an AI and a player controlling the same mech)
        if (GetComponent<PlayerMechControl>())
        {
            Debug.LogError("PlayerMechControl found on this object. Please remove either it or this component.");
        }
        if (salvoDuration > attackRateFar || salvoDuration > attackRateNear)
        {
            Debug.LogWarning("Salvo duration is longer than one of the attack rates. This will cause the AI to fire continuously.");
        }
    }

    private void Update()
    {
        target = HandleTargeting();
        if (target != null)
        {
            Debug.Log("Setting target for all weapons");
            mechWeaponManager.SetTargetForAll(target);
        }
        HandleFireControl();
    }

    private GameObject HandleTargeting()
    {
        Collider[] colliders = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, targetingRangeFar, colliders, playerMask);
        if (colliders[0] == null)
        {
            // Debug.Log("No target found");
            return null;
        }
        else
        {
            // Debug.Log("Target found: " + colliders[0].gameObject.name);
        }

        float distance = Vector3.Distance(transform.position, colliders[0].transform.position);
        //TODO: If this is buggy, cast from a custom origin point
        bool hasLineOfSight = !Physics.Linecast(transform.position, colliders[0].transform.position, ignoreMask);
        // Draw the linecast's line
        Debug.DrawLine(transform.position, colliders[0].transform.position, hasLineOfSight ? Color.green : Color.red);
        // if (!hasLineOfSight)
        // {
        //     Debug.Log("No line of sight");
        // }
        if (distance <= targetingRangeNear && hasLineOfSight)
        {
            //Debug.Log("Targeting near");
            state = AttackState.TargetingNear;
            currentAttackRate = attackRateNear;
        }
        else if (distance <= targetingRangeFar && hasLineOfSight)
        {
            //Debug.Log("Targeting far");
            state = AttackState.TargetingFar;
            currentAttackRate = attackRateFar;
        }
        else
        {
            //Debug.Log("Idle");
            state = AttackState.Idle;
        }

        return state == AttackState.Idle ? null : colliders[0].gameObject;
    }

    private void HandleFireControl()
    {

        // If we have a target, and we are not firing, and the cooldown has passed, start firing
        if (!isFiring && target != null && Time.time - lastAttackTime >= currentAttackRate)
        {
            // Debug.Log("Starting fire");
            // Debug.Log("Cooldown has passed" + Time.time + " - " + lastAttackTime + " >= " + currentAttackRate);
            // Debug.Log("Target is not null");
            // Debug.Log("isFiring is false");
            isFiring = true;
            float random = UnityEngine.Random.Range(0f, 1f);
            if (random <= accuracy) Debug.Log("Firing with perfect accuracy");
            else Debug.Log("Firing with imperfect accuracy");
            mechWeaponManager.StartAllWeapons(random <= accuracy);
            salvoEndTime = Time.time + salvoDuration;
            lastAttackTime = Time.time;
            return;
        }
        // If we lose the target, are in idle, or the salvo duration has passed, stop firing
        if (target == null || Time.time >= salvoEndTime || state == AttackState.Idle)
        {
            // Debug.Log("Stopping fire");
            // if (target == null) Debug.Log("Target is null");
            // if (Time.time >= salvoEndTime) Debug.Log("Salvo duration has passed:" + Time.time + " >= " + salvoEndTime);
            // if (state == AttackState.Idle) Debug.Log("State is idle");
            isFiring = false;
            mechWeaponManager.StopAllWeapons();
            return;
        }

    }
}
