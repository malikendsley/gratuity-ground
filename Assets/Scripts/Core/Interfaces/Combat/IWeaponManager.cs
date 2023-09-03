using System;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public interface IWeaponManager
    {
        // Register and unregister weapons
        void RegisterWeapon(IWeapon weapon, int slot);
        void UnregisterWeapon(int slot);

        // Query for available weapons and slots
        Dictionary<int, IWeapon> GetAvailableWeapons();

        // Fire individual weapons
        void StartWeapon(int slot, bool shouldAimAssist = false);
        void StopWeapon(int slot);

        // Aggregate commands
        void StartAllWeapons(bool shouldAimAssist = false);
        void StopAllWeapons();

        // Target management
        void SetTargetForAll(GameObject target);

        // Events
        event Action<int> OnWeaponRegistered;  // int represents slot
        event Action<int> OnWeaponUnregistered;  // int represents slot
    }
}