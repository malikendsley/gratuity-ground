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
}