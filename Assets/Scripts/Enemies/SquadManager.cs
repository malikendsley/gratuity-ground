using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class SquadManager : MonoBehaviour
    {
        public static SquadManager Instance { get; private set; }

        // The highest level manager for all enemies in the game. Note that all 
        // aspects of spawning and managing enemies should be handled by this class
        // or a class it manages, because there is nowhere else that will.
        public List<Squad> squads = new();

        public GameObject debugSpawnLocation;
        public int defaultSquadSize = 5;
        //NOTE: maybe make defaultSquadSpread a function of size? 
        public float defaultSquadSpread = 10f;
        // TODO: scriptable object that determines spawn characteristics
        // TODO: in the update loop, continuously:
        // - Spawn new squads on a cooldown tied to difficulty
        // - Update the targeting of all squads
        // - Update the movement of all squads
        // - etc.

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

        private void OnDrawGizmos()
        {
            // Draw a circle that represents the spawn area
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugSpawnLocation.transform.position, defaultSquadSpread);
            // Draw a sphere at the spawn location
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(debugSpawnLocation.transform.position, 2.5f);
        }

        private void Update()
        {
            // Existing update logic here
        }

        // Spawn a new squad at the given location
        public void SpawnNewSquad()
        {
            // TODO: make the player controller a singleton to make it easier to access
            // Decide the spawn location. The criteria:
            // - The location should be within the bounds of the map
            // - The location should be far enough away from the player
            // - The location should be far enough away from other squads
            // - The location should not have line of sight to the player
            // Instantiate a new squad object and initialize it with the given location
            // The squad will take over spawning its own enemies, and return a squad object when it is ready

            // For now, use the public debug spawn location and spawn characteristics
            squads.Add(new(debugSpawnLocation.transform.position, defaultSquadSpread, defaultSquadSize, true));
        }

        private IEnumerator SpawnSquadCooldown()
        {
            // Spawn a new squad (details tbd)
            // Yield for an amount of time based on time passed and number of squads
            // Provisionally, increase the squad cap by 1 every minute
            // The squad spawning rate is a function of max number of enemies (and by extension squads) and current live enemies
            // Spawn faster when there are fewer enemies and vice versa
            // TODO: make a difficulty scaling config
            yield return null;
        }
    }
}
