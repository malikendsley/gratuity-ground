using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    //TODO: For now, I only need positions, but this will likely become the 
    // aggregate class for all enemy metadata for performance reasons
    public class EnemyPositionTracker : MonoBehaviour
    {

        public static EnemyPositionTracker Instance { get; private set; }

        [SerializeField] private List<Transform> enemyTransforms = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Start()
        {
            StartCoroutine(CheckForNullTransforms());
        }
        // TODO: Enemies should not disappear without the chance to unregister themselves,
        // but in case that happens we can catch it here
        private IEnumerator CheckForNullTransforms()
        {
            while (true)
            {
                int numTransforms = enemyTransforms.Count;
                enemyTransforms.RemoveAll(t => t == null);
                if (numTransforms != enemyTransforms.Count)
                {
                    Debug.LogWarning("Removed null transform");
                }
                yield return new WaitForSeconds(1f); // Check every 1 second
            }
        }
        public void AddEnemy(Transform enemyTransform)
        {
            enemyTransforms.Add(enemyTransform);
        }

        public void RemoveEnemy(Transform enemyTransform)
        {
            enemyTransforms.Remove(enemyTransform);
        }

        public IList<Transform> GetEnemyTransforms()
        {
            return enemyTransforms.AsReadOnly();
        }
    }
}