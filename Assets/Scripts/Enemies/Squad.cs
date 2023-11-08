using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class Squad
    {

        // Contains the relevant metadata and aggregate functions for a squad of enemies
        readonly List<MechController> members = new();
        readonly List<MovementAI> movementAIs = new();
        private readonly float squadSpread;
        // Initialize a new squad of size members centered around location spread out over a radius of spread
        public Squad(GameObject enemyPrefab, Vector3 location, float spread, int size, Transform enemiesFolder, bool debugDraw = false) // TODO: List<EnemyType> types / SquadLayout layout maybe?
        {
            Debug.Log("Constructor: Squad with debugDraw = " + debugDraw + " created.");
            //NOTE: If spawning consistently fails, maybe try requesting more spawn slots than needed
            // Decide where to spawn
            // HACK: MinDist is set to 10f for now, but it should be a function of the size of the enemies in the squad
            // or otherwise configured thoughtfully
            List<Vector3> spawnPoints = NavMeshUtils.GetNavMeshPoints(location, spread, size, 10f, 100, debugDraw);
            squadSpread = spread;
            // If there are fewer spawn points than size, raise an error
            if (spawnPoints.Count < size)
            {
                Debug.LogError("Not enough spawn points for the requested squad size.");
            }
            var id = 1;
            foreach (Vector3 spawnPoint in spawnPoints)
            {
                // Spawn the enemies, maybe from some sort of enemy spawn manager
                GameObject enemy = Object.Instantiate(enemyPrefab, spawnPoint, Quaternion.identity, enemiesFolder);
                // Rename the enemy to make it easier to find in the hierarchy
                enemy.name = "Enemy " + id++;
                // register the enemy with the enemy position tracker
                //HACK: Formalize access to the CoM of the enemy (since the gameobject's center is in the feet)
                if (enemy.transform.Find("CoM") == null)
                {
                    Debug.LogError("Enemy prefab does not have a CoM child.");
                }
                else
                {
                    // HACK: Proper usage of the enemy position tracker requires that 
                    // all spawned enemies are registered with it. It would make more
                    // sense to place the registration logic closer to the spawn logic
                    // in the future, there may be a factory for enemies that handles
                    // this logic
                    EnemyPositionTracker.Instance.AddEnemy(enemy.transform.Find("CoM"));
                }
                // This enemy is guaranteed to have a MechController and a MovementAI
                if (enemy.TryGetComponent(out MechController mechController))
                {
                    members.Add(mechController);
                }
                else
                {
                    Debug.LogError("Enemy prefab does not have a MechController.");
                }
                if (enemy.TryGetComponent(out MovementAI movementAI))
                {
                    movementAIs.Add(movementAI);
                }
                else
                {
                    Debug.LogError("Enemy prefab does not have a MovementAI.");
                }
                Debug.Log("Spawning enemy at " + spawnPoint);
            }
        }

        public void SetWaypoint(Vector3 destination)
        {
            Debug.Log("Squad: Waypoint received from SquadManager.");
            // HACK: Same hack as in Squad constructor
            List<Vector3> destinations = NavMeshUtils.GetNavMeshPoints(destination, squadSpread, members.Count, 10f, 100);
            // HACK: Centralize access to all controllers instead of specifically touching movement AI here
            foreach (MovementAI movementAI in movementAIs)
            {
                if (!movementAI.MoveToTarget(destinations[movementAIs.IndexOf(movementAI)]))
                {
                    Debug.LogWarning("MovementAI failed to move to target.");
                }
            }
        }
    }
}