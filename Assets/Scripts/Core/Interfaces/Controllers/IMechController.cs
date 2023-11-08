using System;
using UnityEngine;

namespace Endsley
{

    public interface IMechController
    {
        // SO
        MechLocomotionConfig LocomotionConfig { get; }

        // The control vector is a vector within the unit circle representing
        // Both the direction to move in and the magnitude of the movement
        // a mag of 1 is full speed, 0 is no movement
        void UpdateControl(Vector2 controlVector);
        void StopMoving();

        // Jumping
        void Jump();


        // Status queries    
        Vector3 Position { get; }
        float CurrentSpeed { get; }
        bool IsGrounded { get; }

        // // Advanced
        // void MoveToPoint(Vector3 point); // For AI pathing

        // Events for external components to subscribe
        event Action<float> OnSpeedChange;
        event Action OnJump;
        event Action<bool> OnGroundedChange;
    }

}



