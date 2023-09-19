using Endsley;
using UnityEngine;
using System;
using System.Collections.Generic;
// Simple independent AI that automatically attacks a set or acquired target based on distance.
public class CombatAI : MonoBehaviour
{
    //TODO: Convert this into a config like an "attack profile"
    [Tooltip("Objects using this layer are part of the player's mech.")]
    public LayerMask playerMask;
    [Tooltip("Layers to use when checking for line of sight")]
    public LayerMask obstacleMask;
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
    [Tooltip("The required un-occulded volume fraction of the target to be considered a valid target.")]
    [Range(0f, 1f)]
    public float targetVolumeThreshold = 0.5f;
    private GameObject target;
    private MechWeaponManager mechWeaponManager;
    private AttackState state;
    private float lastAttackTime = 0f;
    private float currentAttackRate;
    private bool isFiring = false;
    private float salvoEndTime = 0f;
    private Transform CoM;
    private GameObject lastTarget;


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
        // Find an object labeled CoM in this object's children
        CoM = transform.Find("CoM");
    }

    private void Update()
    {
        target = HandleTargeting();
        HandleState();
        if (target != lastTarget)
        {
            mechWeaponManager.SetTargetForAll(target);
        }
        HandleFireControl();
        lastTarget = target;
    }

    // Returns the target if one is found, otherwise null
    private GameObject HandleTargeting()
    {
        // Initially check if we can just see the player
        if (!Physics.Linecast(CoM.position, PlayerMechTag.Instance.PlayerCoM.transform.position, obstacleMask))
        {
            return PlayerMechTag.Instance.PlayerCoM;
        }

        // Otherwise, try to target the largest section of the player if enough of it is visible
        List<Collider> playerColliders = new(Physics.OverlapSphere(CoM.position, targetingRangeFar, playerMask));
        float totalVolume = 0f;
        float unobstructedVolume = 0f;
        List<Collider> unobstructedColliders = new();

        foreach (Collider collider in playerColliders)
        {
            float colliderVolume = Volume(collider);
            totalVolume += colliderVolume;

            if (!Physics.Linecast(CoM.position, collider.bounds.center, obstacleMask))
            {
                unobstructedVolume += colliderVolume;
                unobstructedColliders.Add(collider);

                if (unobstructedVolume / totalVolume >= targetVolumeThreshold)
                {
                    Debug.Log("Setting target to " + unobstructedColliders[0].gameObject.name);
                    Debug.DrawLine(CoM.position, collider.bounds.center, Color.green);
                    return unobstructedColliders[0].gameObject;
                }
            }
        }

        return null;
    }

    private float Volume(Collider collider)
    {
        Vector3 size = collider.bounds.size;
        return size.x * size.y * size.z;
    }
    // based on the current target, determine the state
    // distance > targetingrangefar = idle
    // targetingrangefar > distance > targetingrangenear = targetingfar
    // targetingrangenear > distance = targetingnear
    private void HandleState()
    {
        if (target == null)
        {
            Debug.Log("No target");
            state = AttackState.Idle;
            return;
        }
        float distance = Vector3.Distance(CoM.position, target.transform.position);
        if (distance > targetingRangeFar)
        {
            Debug.Log("Target out of range");
            state = AttackState.Idle;
            target = null;
            return;
        }
        if (distance > targetingRangeNear)
        {
            Debug.Log("Target in far range");
            state = AttackState.TargetingFar;
            currentAttackRate = attackRateFar;
            return;
        }
        Debug.Log("Target in near range");
        state = AttackState.TargetingNear;
        currentAttackRate = attackRateNear;
    }

    private void HandleFireControl()
    {


        // If we have a target, and we are not firing, and the cooldown has passed, start firing
        if (!isFiring && target != null && Time.time - lastAttackTime >= currentAttackRate)
        {
            Debug.Log("Starting weapons to fire at target: " + target.name);
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
            isFiring = false;
            mechWeaponManager.StopAllWeapons();
            return;
        }
    }
}
