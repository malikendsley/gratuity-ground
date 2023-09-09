using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Endsley
{
    public static class NavMeshUtils
    {
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

    }
}