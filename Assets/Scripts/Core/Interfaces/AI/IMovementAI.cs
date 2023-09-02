using UnityEngine;

namespace Endsley
{
    public interface IMovementAI
    {
        bool MoveToTarget(Vector3 target);
        void SetActive(bool active);
    }
}