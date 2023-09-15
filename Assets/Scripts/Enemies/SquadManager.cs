using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This entire class may not be necessary, but I don't know where else to put the squad spawning logic besides a game manager
// Super object that manages all aspects of the game
namespace Endsley
{
    public class SquadManager : MonoBehaviour
    {
        public static SquadManager Instance { get; private set; }

        // The highest level manager for all enemies in the game. Note that all 
        // aspects of spawning and managing enemies should be handled by this class
        // or a class it manages, because there is nowhere else that will.
        readonly List<Squad> squads = new();
        public GameObject enemiesFolder;
        public int maxSquadCount = 5;
        public GameObject enemyPrefab;
        public int defaultSquadSize = 5;
        public float defaultSquadSpread = 10f;
        public float playerMinSpawnDistance = 50f;
        public float playerMaxSpawnDistance = 50f;
        public float playerWaypointMinDistance = 5f;
        public float playerWaypointMaxDistance = 10f;
        public float cooldownTime = 10f;
        private float timer;
        public bool canSpawn = false;
        public bool spawningAllowed = false;
        public LayerMask LOSMask;

        [Tooltip("How often to update the waypoints of all squads in updates / second.")]
        [SerializeField] private float squadWaypointUpdateFrequency = .1f;
        private float squadWaypointUpdateTimer = 0f;
        private float nextSquadWaypointUpdateTime = 0f;

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
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(PlayerMechControl.Instance.PlayerTransform.position, playerMinSpawnDistance);
            }
        }
        private void Start()
        {
            Debug.Log("NOTE: Canspawn is set to false in SquadManager.Start(). Set this to true later.");
            timer = cooldownTime;
            StartCoroutine(SpawnSquadCooldown());
        }

        private void Update()
        {

            squadWaypointUpdateTimer += Time.deltaTime;
            if (squadWaypointUpdateTimer >= nextSquadWaypointUpdateTime)
            {
                foreach (var squad in squads)
                {
                    // HACK: Properly parameterize this
                    Vector3 newWaypoint = PlayerMechControl.Instance.GetNearbyPoint(playerWaypointMinDistance, playerWaypointMaxDistance);
                    squad.SetWaypoint(newWaypoint);
                }

                nextSquadWaypointUpdateTime = 1f / squadWaypointUpdateFrequency;
                squadWaypointUpdateTimer = 0f;
            }
        }

        // Spawn a new squad on the map
        public void SpawnNewSquad()
        {
            if (squads.Count >= maxSquadCount)
            {
                Debug.Log("Max squad count reached, cannot spawn squad.");
                return;
            }
            Vector3 potentialLocation = NavMeshUtils.GetRandomNavMeshPoint();
            DebugUtils.DrawTempDebugSphere(potentialLocation, 5f, 2f, Color.cyan);
            if (!GoodLocationToSpawn(potentialLocation, Camera.main))
            {
                return;
            }
            else
            {
                Debug.Log("No line of sight to player, spawning squad.");
                // TODO: turn off debug draw
                Squad newSquad = new(enemyPrefab, potentialLocation, defaultSquadSpread, defaultSquadSize, enemiesFolder.transform, true);
                squads.Add(newSquad);
            }
        }

        // HACK: Clean this mess up
        private bool GoodLocationToSpawn(Vector3 point, Camera camera)
        {
            // Check if point is within the camera's view
            Vector3 viewportPoint = camera.WorldToViewportPoint(point);
            bool isInView = viewportPoint.z > 0 && viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1;

            // If it's in view, return false
            if (isInView)
            {
                Debug.Log("Point is in view, returning false");
                return false;
            }
            else
            {
                // If the distance is greater than playminDistance, return true
                if (Vector3.Distance(point, PlayerMechControl.Instance.PlayerTransform.position) > playerMinSpawnDistance)
                {
                    Debug.Log("Point is out of view and out of range, returning true");
                    return true;
                }
            }

            // Perform raycasting to check if there's any obstruction
            bool hasLineOfSight = !Physics.Raycast(camera.transform.position, point - camera.transform.position, Vector3.Distance(camera.transform.position, point), LOSMask);

            // If it's out of view and not obstructed, return true
            Debug.Log("Point is in view: " + isInView + ", has line of sight: " + hasLineOfSight + ", returning " + !hasLineOfSight + ".");
            return hasLineOfSight;
        }
        // TODO: de-enumify this
        private IEnumerator SpawnSquadCooldown()
        {
            // TODO: make a difficulty scaling config

            while (true)
            {
                if (spawningAllowed)
                {
                    if (canSpawn)
                    {
                        Debug.Log("SquadManager: Spawning new squad");
                        SpawnNewSquad();

                        canSpawn = false;
                        timer = cooldownTime;
                    }
                    else
                    {
                        timer = Math.Max(timer - 1f, 0f);
                        Debug.Log("Remaining cooldown: " + timer + "s");

                        if (timer <= 0)
                        {
                            canSpawn = true;
                        }
                    }
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }
}
