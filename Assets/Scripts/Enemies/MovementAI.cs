using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Endsley
{
    public class MovementAI : MonoBehaviour, IMovementAI
    {
        private bool active = true;
        private Vector3 target;
        //TODO: Make this a config
        public float angleThreshold = 2f;
        public float distanceThreshold = 1f;
        public float skipDistance = 3f;
        public float navMeshSampleDistance = 5f;
        [Tooltip("If enabled, will stop moving if its forward direction is blocked by another mech")]
        [SerializeField] private bool waitForOtherMechs = true;
        [Tooltip("Distance to check for other mechs.")]
        [SerializeField] private float waitForOtherMechsDistance = 5f;
        [Tooltip("Angle in degrees to check for other mechs. 0 means only check directly in front of the mech.")]
        [SerializeField] private float waitForOtherMechsAngle = 30f;
        private SphereCollider mechCheckSphere;
        private NavMeshAgent agent;
        private NavMeshPath path;
        private List<Vector3> corners;
        private int currentCorner = 0;
        private IMechController mechController;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, waitForOtherMechsDistance);
            //Draw a wire arc in front of the object representing the angle
            Gizmos.DrawWireSphere(transform.position, distanceThreshold);
        }

        void Start()
        {
            if (TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                agent = navMeshAgent;
            }
            else
            {
                Debug.LogWarning("No NavMeshAgent found on this object. AI will not be able to move.");
            }
            path = new NavMeshPath();
            corners = new List<Vector3>();
            // Find the mech controller on this object
            if (TryGetComponent(out MechController controller))
            {
                mechController = controller;
            }
            else
            {
                Debug.LogWarning("No MechController found on this object. AI will not be able to move.");
            }
            // Create a collider on this object to check for other mechs
            mechCheckSphere = gameObject.AddComponent<SphereCollider>();
            mechCheckSphere.isTrigger = true;
            mechCheckSphere.radius = waitForOtherMechsDistance;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!waitForOtherMechs || other.gameObject == gameObject)
            {
                return;
            }
            // Check if the other collider is a mech
            if (other.TryGetComponent(out MechController _))
            {
                // Check if the other mech is in front of this mech
                Vector3 directionToOtherMech = other.transform.position - transform.position;
                float angleToOtherMech = Vector3.SignedAngle(transform.forward, directionToOtherMech, Vector3.up);
                if (Mathf.Abs(angleToOtherMech) <= waitForOtherMechsAngle)
                {
                    // Stop moving if the other mech is in front of this mech
                    active = false;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Don't check waitForOtherMechs in case we changed the setting while the other mech was in the trigger
            // Check if the other collider is a mech
            if (other.TryGetComponent(out MechController _))
            {
                // Check if the other mech is in front of this mech
                Vector3 directionToOtherMech = other.transform.position - transform.position;
                float angleToOtherMech = Vector3.SignedAngle(transform.forward, directionToOtherMech, Vector3.up);
                if (Mathf.Abs(angleToOtherMech) <= waitForOtherMechsAngle)
                {
                    // Resume moving if the other mech is no longer in front of this mech
                    active = true;
                }
            }
        }

        void Update()
        {
            if (active && corners.Count > 0)
            {

                for (int i = 0; i < corners.Count - 1; i++)
                {
                    Debug.DrawLine(corners[i], corners[i + 1], Color.red);
                }
                Debug.DrawRay(corners[currentCorner], Vector3.up * 0.5f, Color.blue);

                Vector3 nextPoint = corners[currentCorner];
                Debug.Log("Moving to " + nextPoint + "(remaining distance: " + Vector3.Distance(transform.position, nextPoint) + ")");
                Vector3 directionToNextPoint = nextPoint - transform.position;
                mechController.UpdateControl(new Vector2(directionToNextPoint.x, directionToNextPoint.z));

                // Check if reached the current corner or if it's too close to skip
                if (Vector3.Distance(transform.position, nextPoint) < distanceThreshold)
                {
                    Debug.Log("Reached corner");
                    currentCorner++;
                    if (currentCorner >= corners.Count)
                    {
                        Debug.Log("Clearing path");
                        corners.Clear();  // Clear path when reached the end
                        currentCorner = 0;
                        mechController.StopMoving();
                    }
                }

            }
            else
            {
                Debug.Log("Not moving: active = " + active + ", corners.Count = " + corners.Count);
                mechController.StopMoving();
            }
        }

        // Attempts to move to the target position
        // Returns true if a path was found, false otherwise
        public bool MoveToTarget(Vector3 transform)
        {
            Debug.Log("MovementAI: destination set by Squad.");
            target = transform;

            if (NavMesh.SamplePosition(target, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                if (agent.CalculatePath(hit.position, path))
                {
                    corners.Clear();
                    corners.AddRange(path.corners);
                    currentCorner = 0;
                    Debug.Log("Path calculated (" + corners.Count + " corners).");
                }
                else
                {
                    Debug.LogWarning("Path could not be calculated.");
                    return false;
                }
                return true;
            }
            else
            {
                Debug.LogWarning("Target position could not be sampled on NavMesh.");
                return false;

            }
        }

        public void SetActive(bool active)
        {
            this.active = active;
        }
    }
}
