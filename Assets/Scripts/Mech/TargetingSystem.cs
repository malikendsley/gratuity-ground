using System;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class TargetingSystem : MonoBehaviour
    {
        [Tooltip("When true, the player can target enemies by aiming within aimTargetingAngle of them.")]
        [SerializeField] bool aimTargeting = true;
        [SerializeField] float aimTargetingAngle = 15f;
        [SerializeField] LayerMask occlusionLayerMask;
        private GameObject playerTarget;
        private GameObject lastTarget;
        private WeaponsBus weaponsBus;
        private void Start()
        {
            playerTarget = null;
            lastTarget = null;
            if (EnemyPositionTracker.Instance == null)
            {
                Debug.LogError("EnemyPositionTracker not found in the scene. Please add one.");
            }
            weaponsBus = WeaponsBusManager.Instance.GetOrCreateBus(PlayerMechTag.Instance.PlayerMech);
        }

        private void Update()
        {
            bool enemyTargeted = false;
            if (playerTarget)
            {
                Debug.DrawLine(Camera.main.transform.position, playerTarget.transform.position, Color.magenta);
            }
            if (aimTargeting)
            {
                List<Transform> enemyTransforms = new(EnemyPositionTracker.Instance.GetEnemyTransforms());
                foreach (Transform enemyTransform in enemyTransforms)
                {
                    Vector3 cameraForward = Camera.main.transform.forward;
                    Vector3 playerToEnemy = enemyTransform.position - Camera.main.transform.position;
                    float angle = Vector3.Angle(playerToEnemy, cameraForward);

                    if (angle < aimTargetingAngle)
                    {
                        if (!Physics.Raycast(Camera.main.transform.position, playerToEnemy, out RaycastHit hit, Vector3.Distance(Camera.main.transform.position, enemyTransform.position), occlusionLayerMask))
                        {
                            enemyTargeted = true;
                            playerTarget = enemyTransform.gameObject;
                            break;
                        }
                    }
                }
            }

            if (!enemyTargeted)
            {
                playerTarget = null;
            }

            if (playerTarget != lastTarget)
            {
                Debug.Log(playerTarget != null ? $"Target is now {playerTarget.name}" : "Target is now null");
                weaponsBus.Emit(playerTarget ? WeaponEventType.OnTargetChange : WeaponEventType.OnTargetClear, new WeaponEventData { Target = playerTarget ? playerTarget : null });

                lastTarget = playerTarget;
            }
        }

        public GameObject GetPlayerTarget()
        {
            return playerTarget;
        }

        public void SetPlayerTarget(GameObject target)
        {
            playerTarget = target;
        }

        public void ClearPlayerTarget()
        {
            playerTarget = null;
        }

    }
}