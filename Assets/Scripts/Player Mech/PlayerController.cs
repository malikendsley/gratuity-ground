using UnityEngine;
using UnityEngine.InputSystem;

namespace Endsley
{
    public class PlayerController : MonoBehaviour
    {
        private MechController mechController;

        private void Awake()
        {
            mechController = GetComponent<MechController>();
        }

        // Additional player-specific input handling
    }
}