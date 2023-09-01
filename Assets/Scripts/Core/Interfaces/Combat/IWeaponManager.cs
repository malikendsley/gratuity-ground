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
        void FireWeapon(int slot, bool shouldAimAssist = false);
        void PrepWeapon(int slot);
        void StopWeapon(int slot);

        // Aggregate commands
        void FireAllWeapons();
        void PrepAllWeapons();
        void StopAllWeapons();

        // Target management
        void SetTargetForAll(GameObject target);

        // Events
        event Action<int> OnWeaponRegistered;  // int represents slot
        event Action<int> OnWeaponUnregistered;  // int represents slot
    }
}