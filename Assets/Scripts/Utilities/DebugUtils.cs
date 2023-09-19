using UnityEngine;
namespace Endsley
{
    public static class DebugUtils
    {
        public static void DrawTempDebugSphere(Vector3 position, float scale = 0.5f, float duration = 2f, Color? color = null)
        {
            GameObject debugDot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            debugDot.transform.position = position;
            debugDot.transform.localScale = new Vector3(scale, scale, scale);

            if (color.HasValue)
            {
                debugDot.GetComponent<Renderer>().material.color = color.Value;
            }

            Object.Destroy(debugDot, duration);
        }

        public static void DrawTempDebugLine(Vector3 start, Vector3 end, float duration = 2f, Color? color = null)
        {
            GameObject debugLine = new GameObject("DebugLine");
            debugLine.transform.position = start;
            debugLine.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = debugLine.GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            if (color.HasValue)
            {
                lineRenderer.material.color = color.Value;
            }

            Object.Destroy(debugLine, duration);
        }
    }
}