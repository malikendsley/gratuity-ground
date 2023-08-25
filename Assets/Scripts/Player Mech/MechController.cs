using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MechController : MonoBehaviour
{

    // Other components
    [SerializeField]
    private MechLocomotionConfig locomotionConfig;
    [SerializeField]
    private CharacterController characterController;

    // Locomotion
    private float targetSpeed = 0f;
    private float currentSpeed = 0f;
    private Vector2 movementVector = Vector2.zero;

    // Input TODO: Remove this and put it in its own script
    [SerializeField]
    private InputAction playerControls;

    // Actions for external users
    public event Action<float> OnSpeedChange;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        // TODO: Refactor this and separate input from mech movement
        // Gather input
        movementVector = playerControls.ReadValue<Vector2>();
        Debug.Log("Movement vector: " + movementVector);

        //TODO: Y needs to be flipped because for some reason the animations are backwards on the mech due to the way the bones were handled
        movementVector.y = -movementVector.y;

        SetSpeed(movementVector);
        //Linearly interpolate the character controller's current speed to target speed
        float acceleration = targetSpeed == 0f ? locomotionConfig.deceleration : locomotionConfig.acceleration;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        Vector3 movement = currentSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
        Debug.Log("Calling Move with: " + movement);
        characterController.Move(movement);

        Rotate(movementVector);
    }

    public void SetSpeed(Vector2 movementVector)
    {
        if (movementVector.y > 0)
        {
            targetSpeed = locomotionConfig.walkSpeed;
        }
        else if (movementVector.y < 0)
        {
            targetSpeed = -locomotionConfig.walkSpeed;
        }
        else
        {
            targetSpeed = 0;
        }
    }

    public void Rotate(Vector2 movementVector)
    {
        float rotationAmount = movementVector.x * locomotionConfig.rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAmount, 0);
    }

    //Point the gun towards the target (TODO: should this take the target location, or a direction to look? Maybe override?)
    public void Look(Vector2 input)
    {
        // Implementation of the look function
    }


    //TODO: Use these to drive the UI later
    //Expose the rotation and speed values
    public int GetLegsRotation()
    {
        //Get the smallest angle in degrees from the original forward vector and the current legs rotation
        Debug.LogError("Not implemented yet!");
        return 0;
    }

    public int GetHeadRotation()
    {
        //Get the smallest angle in degrees from the original forward vector and the current head rotation
        Debug.LogError("Not implemented yet!");
        return 0;
    }

}