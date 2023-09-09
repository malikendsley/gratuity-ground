using Endsley;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance; // Static instance
    public GameObject enemyPrefab;
    public MechLocomotionConfig locomotionConfig;
    // TODO: Rest of configs here (health, combat, etc.)

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

    public GameObject SpawnEnemy(Vector3 position, Squad squad)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // With the enemy and the squad available, should be able to set everything up

        return enemy;
    }
}
