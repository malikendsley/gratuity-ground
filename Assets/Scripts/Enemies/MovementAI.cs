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
        private NavMeshAgent agent;
        private NavMeshPath path;
        private List<Vector3> corners;
        private int currentCorner = 0;
        private MechController mechController;
        // //HACK: make this inline again this after testing
        // [SerializeField] private float angleToNextPoint;
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
        }
        // void OnDrawGizmos()
        // {
        //     if (active && corners != null && corners.Count > 0 && currentCorner < corners.Count)
        //     {
        //         // Draw a blue sphere at the immediate next point
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawSphere(corners[currentCorner], 0.5f);
        //     }
        // }
        void Update()
        {
            if (active && corners.Count > 0)
            {
                // // Draw the path
                for (int i = 0; i < corners.Count - 1; i++)
                {
                    Debug.DrawLine(corners[i], corners[i + 1], Color.red);
                }
                Debug.DrawRay(corners[currentCorner], Vector3.up * 0.5f, Color.blue);

                Vector3 nextPoint = corners[currentCorner];
                Vector3 directionToNextPoint = nextPoint - transform.position;
                //HACK: If one day this breaks by going away from the target, un-negate this
                float angleToNextPoint = Vector3.SignedAngle(-transform.forward, directionToNextPoint, Vector3.up);

                // If we're not looking at the target, rotate first
                if (Mathf.Abs(angleToNextPoint) > angleThreshold)
                {
                    //Debug.Log("Rotating to heading: " + angleToNextPoint);
                    mechController.StopMoving();
                    mechController.RotateToTarget(nextPoint);
                }
                // Else, move forward only if heading is correct
                else if (Mathf.Abs(angleToNextPoint) <= angleThreshold)
                {
                    Debug.Log("Heading is correct");
                    mechController.UpdateControl(new(0, 1));
                }

                // Check if reached the current corner or if it's too close to skip
                float distanceToNextPoint = Vector3.Distance(transform.position, nextPoint);
                if (distanceToNextPoint < distanceThreshold || distanceToNextPoint < skipDistance)
                {
                    Debug.Log("Reached or skipped corner");
                    currentCorner++;
                    if (currentCorner >= corners.Count)
                    {
                        corners.Clear();  // Clear path when reached the end
                        currentCorner = 0;
                        mechController.StopMoving();
                    }
                }

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
