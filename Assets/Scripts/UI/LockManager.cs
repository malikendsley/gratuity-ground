using System.Collections.Generic;
using UnityEngine;

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
            tentativeReticleSprite.transform.SetParent(hudCanvas.transform, false);
            tentativeReticleSprite.SetActive(false);

            // HACK: Find a more robust way to do this
            // Get the targeting system from the main camera
            targetingSystem = Camera.main.GetComponent<TargetingSystem>();
            if (!targetingSystem)
            {
                Debug.LogError("TargetingSystem not assigned to the lockmanager. Please add one.");
            }
            else
            {
                // Subscribe to the targeting system's OnTargetChanged event
                targetingSystem.OnTargetChanged += SetCurrentTarget;
                targetingSystem.OnTargetCleared += ClearCurrentTarget;
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
            if (currentTarget)
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
            // Don't draw a reticle for an enemy that's already being tracked
            if (!trackedEnemies.Contains(enemy))
            {
                trackedEnemies.Add(enemy);
                // Instantiate a new reticle sprite at the enemy's position
                Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
                GameObject reticle = Instantiate(sprite, screenPos, Quaternion.identity);
                trackingReticleSprites.Add(reticle);
            }
        }

        public void RemoveEnemy(GameObject enemy)
        {
            int index = trackedEnemies.IndexOf(enemy);
            if (index != -1)
            {
                trackedEnemies.RemoveAt(index);
                Destroy(trackingReticleSprites[index]);
                trackingReticleSprites.RemoveAt(index);
            }
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

    }
}
