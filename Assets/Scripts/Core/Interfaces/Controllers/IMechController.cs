using System;
using UnityEngine;

namespace Endsley
{

    public interface IMechController
    {
        // SO
        MechLocomotionConfig LocomotionConfig { get; }

        // Basic movement
        void UpdateControl(Vector2 control);
        void StopMoving();

        // Rotation
        void RotateToHeading(float targetHeading); // For AI-like discrete rotation

        // Jumping
        void Jump();

        // // Shooting and combat
        // void Shoot();
        // void AimAt(Vector3 target);

        // Status queries    
        Vector3 Position { get; }
        float CurrentSpeed { get; }
        bool IsGrounded { get; }

        // // Advanced
        // void MoveToPoint(Vector3 point); // For AI pathing

        // Events for external components to subscribe
        event Action<float> OnSpeedChange;
        event Action<float> OnRotationChange;
        event Action OnJump;
        event Action<bool> OnGroundedChange;
    }

}



