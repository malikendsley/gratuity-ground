using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private MechController mechController;

    private void Awake()
    {
        mechController = GetComponent<MechController>();
    }

    // Additional player-specific input handling
}