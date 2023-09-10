using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Profiling;

namespace Endsley
{
    public class NavMeshUtils : MonoBehaviour
    {

        private static Bounds navMeshBounds;

        void Awake()
        {
            Profiler.BeginSample("CalculateNavMeshBounds");
            navMeshBounds = CalculateNavMeshBounds();
            Profiler.EndSample();
        }

        private void OnDrawGizmos()
        {
            //Draw the 12 navmesh edges
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.min.z), new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.min.z), new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.min.z), new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.max.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.max.z), new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.max.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.max.z), new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.max.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.max.z), new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.min.z), new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.min.z), new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.max.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.min.x, navMeshBounds.max.y, navMeshBounds.min.z), new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.max.z), new Vector3(navMeshBounds.min.x, navMeshBounds.min.y, navMeshBounds.max.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.max.z), new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.min.z), Color.red);
            Debug.DrawLine(new Vector3(navMeshBounds.max.x, navMeshBounds.min.y, navMeshBounds.max.z), new Vector3(navMeshBounds.max.x, navMeshBounds.max.y, navMeshBounds.max.z), Color.red);

        }
        public static List<Vector3> GetNavMeshPoints(Vector3 center, float radius, int pointCount, float minDist, int maxTries = 0, bool debugDraw = false)
        {
            List<Vector3> points = new();
            int tries = 0;

            for (int i = 0; i < pointCount;)
            {
                Vector2 randomPoint = Random.insideUnitCircle * radius;
                Vector3 randomPosition = center + new Vector3(randomPoint.x, 0, randomPoint.y);

                if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, radius, NavMesh.AllAreas))
                {
                    bool tooClose = false;

                    foreach (Vector3 point in points)
                    {
                        if (Vector3.Distance(point, hit.position) < minDist)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        if (debugDraw)
                        {
                            DebugUtils.DrawTempDebugSphere(hit.position, 2.5f, 2f, Color.green);
                        }
                        points.Add(hit.position);
                        i++;
                    }
                    else if (maxTries > 0 && ++tries >= maxTries)
                    {
                        break;
                    }
                }
                else if (debugDraw)
                {
                    DebugUtils.DrawTempDebugSphere(randomPosition, 2.5f, 2f, Color.red);
                }
            }

            return points;
        }

        public static Bounds CalculateNavMeshBounds()
        {
            NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

            foreach (var vertex in navMeshData.vertices)
            {
                if (vertex.x < minX) minX = vertex.x;
                if (vertex.y < minY) minY = vertex.y;
                if (vertex.z < minZ) minZ = vertex.z;

                if (vertex.x > maxX) maxX = vertex.x;
                if (vertex.y > maxY) maxY = vertex.y;
                if (vertex.z > maxZ) maxZ = vertex.z;
            }

            Vector3 min = new(minX, minY, minZ);
            Vector3 max = new(maxX, maxY, maxZ);
            Vector3 size = max - min;

            return new Bounds((min + max) * 0.5f, size);
        }

        public static Vector3 GetRandomNavMeshPoint()
        {
            Debug.Log("Navmesh X: " + navMeshBounds.min.x + " to " + navMeshBounds.max.x);
            Debug.Log("Navmesh Y: " + navMeshBounds.min.y + " to " + navMeshBounds.max.y);
            Debug.Log("Navmesh Z: " + navMeshBounds.min.z + " to " + navMeshBounds.max.z);
            Vector3 randomPoint = new(
                Random.Range(navMeshBounds.min.x, navMeshBounds.max.x),
                Random.Range(navMeshBounds.min.y, navMeshBounds.max.y),
                Random.Range(navMeshBounds.min.z, navMeshBounds.max.z)
            );

            // The navmesh bounds are set by the different floors, so the max distance will generally be the distance to the nearest floor. 1.25 for a bit of leeway
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, (float)(navMeshBounds.max.y - navMeshBounds.min.y * 1.25), NavMesh.AllAreas))
            {
                DebugUtils.DrawTempDebugSphere(hit.position, 2.5f, 5f, Color.magenta);
                return hit.position;
            }
            Debug.LogWarning("Sampling failed");
            return Vector3.zero; // Error state
        }
    }

}
