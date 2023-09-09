using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class Squad
    {

        // Contains the relevant metadata and aggregate functions for a squad of enemies
        // List of enemies
        List<MechController> members = new();
        // Initialize a new squad of size members centered around location spread out over a radius of spread
        // TODO: squads with custom compositions (for now there is only 1 type of enemy)
        public Squad(Vector3 location, float spread, int size, bool debugDraw = false) // TODO: List<EnemyType> types / SquadLayout layout maybe?
        {
            Debug.Log("Constructor: Squad with debugDraw = " + debugDraw + " created.");
            //NOTE: If spawning consistently fails, maybe try requesting more spawn slots than needed
            // Decide where to spawn
            // HACK: MinDist is set to 10f for now, but it should be a function of the size of the enemies in the squad
            // or otherwise configured thoughtfully
            List<Vector3> spawnPoints = NavMeshUtils.GetNavMeshPoints(location, spread, size, 10f, 100, debugDraw);
            // If there are fewer spawn points than size, raise an error
            if (spawnPoints.Count < size)
            {
                Debug.LogError("Not enough spawn points for the requested squad size.");
            }
            foreach (Vector3 spawnPoint in spawnPoints)
            {
                // Spawn the enemies, maybe from some sort of enemy spawn manager
                //EnemySpawnManager.Instance.SpawnEnemy(spawnPoint, this);
                // TODO: Spawn an enemy at spawnPoint and register it with the squad (exact nature depends on necessary functionality)
            }
            // debug spheres were already drawn when the navmesh was sampled so no need to draw them again

        }

        // What kind of aggregate functions do we want to have?
        // - Get the position of the squad
        // - Get the average health of the squad
        // - Issue commands? (move, attack, etc.)

    }
}