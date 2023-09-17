using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Endsley
{
    public class LockManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> trackedEnemies = new();
        private readonly List<GameObject> trackingReticleSprites = new();
        [SerializeField] private GameObject currentTarget;
        [SerializeField] private GameObject tentativeReticleSprite;
        [SerializeField] private TargetingSystem targetingSystem;
        [SerializeField] private GameObject sprite;
        [SerializeField] private GameObject hudCanvas;

        public static LockManager Instance { get; private set; }
        private WeaponsBus weaponsBus;
        private Action<WeaponEventData> HandleOnLockStack;
        private Action<WeaponEventData> HandleOnWeaponStop;
        private Action<WeaponEventData> HandleOnTargetChange;
        private Action<WeaponEventData> HandleOnTargetClear;
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            tentativeReticleSprite = Instantiate(sprite);
            tentativeReticleSprite.GetComponent<Image>().color = Color.green;

            tentativeReticleSprite.transform.SetParent(hudCanvas.transform, false);
            tentativeReticleSprite.SetActive(false);

            // Grab the weapons bus for the player mech
            weaponsBus = WeaponsBusManager.Instance.GetOrCreateBus(PlayerMechTag.Instance.PlayerMech);
            if (weaponsBus == null)
            {
                Debug.LogError("WeaponsBus not found for player mech. Please add one.");
            }
            else
            {
                // OnLockStack draws (and tracks) new reticles for each enemy that is locked
                // OnWeaponStop clears the tracking reticles
                HandleOnLockStack = (WeaponEventData data) => AddEnemy(data.Target);
                HandleOnWeaponStop = (WeaponEventData data) => ClearAllTargets();
                HandleOnTargetChange = (WeaponEventData data) => SetCurrentTarget(data.Target);
                HandleOnTargetClear = (WeaponEventData data) => ClearCurrentTarget();
                weaponsBus.Subscribe(WeaponEventType.OnLockStack, HandleOnLockStack);
                weaponsBus.Subscribe(WeaponEventType.OnWeaponStop, HandleOnWeaponStop);
                weaponsBus.Subscribe(WeaponEventType.OnTargetChange, HandleOnTargetChange);
                weaponsBus.Subscribe(WeaponEventType.OnTargetClear, HandleOnTargetClear);
            }
        }


        private void Update()
        {
            // For each tracked enemy, draw a reticle sprite over their position, mapped to the screen
            for (int i = 0; i < trackedEnemies.Count; i++)
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(trackedEnemies[i].transform.position);
                trackingReticleSprites[i].transform.position = screenPos;
            }

            // If there's a current target, draw a reticle sprite over their position, mapped to the screen
            // We are using a rect transform
            if (currentTarget && !trackedEnemies.Contains(currentTarget))
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(currentTarget.transform.position);
                tentativeReticleSprite.transform.position = screenPos;
                tentativeReticleSprite.SetActive(true);
            }
            else
            {
                tentativeReticleSprite.SetActive(false);
            }
        }

        public void AddEnemy(GameObject enemy)
        {
            Debug.Log("LockManager: Adding enemy " + enemy.name);
            // Don't draw a reticle for an enemy that's already being tracked
            if (!trackedEnemies.Contains(enemy))
            {
                trackedEnemies.Add(enemy);
                // Instantiate a new reticle sprite at the enemy's position
                Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
                GameObject reticle = Instantiate(sprite);
                // Change the tint of the reticle to blue
                reticle.GetComponent<Image>().color = Color.red;
                reticle.transform.SetParent(hudCanvas.transform, false);
                reticle.transform.position = screenPos;
                trackingReticleSprites.Add(reticle);
            }
        }

        public void RemoveEnemy(GameObject enemy)
        {
            Debug.Log("LockManager: Removing enemy " + enemy.name);
            int index = trackedEnemies.IndexOf(enemy);
            if (index != -1)
            {
                trackedEnemies.RemoveAt(index);
                Destroy(trackingReticleSprites[index]);
                trackingReticleSprites.RemoveAt(index);
            }
        }

        public void ClearAllTargets()
        {
            Debug.Log("LockManager: Clearing all targets");
            trackedEnemies.Clear();
            foreach (GameObject reticle in trackingReticleSprites)
            {
                Destroy(reticle);
            }
            trackingReticleSprites.Clear();
        }

        public void SetCurrentTarget(GameObject enemy)
        {
            Debug.Log("LockManager: Setting current target to " + enemy.name);
            currentTarget = enemy;
        }

        public void ClearCurrentTarget()
        {
            Debug.Log("LockManager: Clearing current target");
            currentTarget = null;
        }
        private void OnDestroy()
        {
            if (weaponsBus != null)
            {
                weaponsBus.Unsubscribe(WeaponEventType.OnLockStack, HandleOnLockStack);
                weaponsBus.Unsubscribe(WeaponEventType.OnWeaponStop, HandleOnWeaponStop);
            }
        }
    }
}
