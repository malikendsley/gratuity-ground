using UnityEngine;

namespace Endsley
{
    public static class UtilScripts
    {
        public static Transform GetRootTransform(Transform child)
        {
            Transform currentParent = child;
            while (currentParent.parent)
            {
                currentParent = currentParent.parent;
            }
            return currentParent;
        }

    }

    public static class ScreenRaycastUtil
    {
        public static Transform GetScreenCenterRaycastHit()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return null;

            Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.transform;
            }

            return null;
        }

        public static Vector3 GetScreenCenterToWorldPoint(float distance)
        {
            Camera mainCam = Camera.main;
            if (mainCam == null) return Vector3.zero;

            Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            return ray.GetPoint(distance);
        }

        public static GameObject GetScreenCenterToWorldPointObject(float distance)
        {
            Vector3 worldPoint = GetScreenCenterToWorldPoint(distance);
            GameObject tempObj = new("TemporaryTarget");
            tempObj.transform.position = worldPoint;
            return tempObj;
        }
    }

}