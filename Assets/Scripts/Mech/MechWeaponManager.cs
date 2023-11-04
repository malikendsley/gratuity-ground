using System;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class MechWeaponManager : MonoBehaviour, IWeaponManager
    {

        readonly Dictionary<int, IWeapon> weapons = new();
        public event Action<int> OnWeaponRegistered;
        public event Action<int> OnWeaponUnregistered;
        //ID of the next weapon if the weaponManager is allowed to handle it
        private int next = 1;

        public void StartAllWeapons(bool shouldAimAssist = false)
        {
            foreach (var item in weapons)
            {
                weapons[item.Key].StartWeapon(shouldAimAssist);
            }
        }

        public void StartWeapon(int slot, bool shouldAimAssist = false)
        {
            Debug.Log("Firing weapon in slot " + slot);
            if (CheckWeaponSlot(slot))
            {
                weapons[slot].StartWeapon(shouldAimAssist);
            }
            else
            {
                Debug.LogWarning($"No weapon registered at slot {slot}");
            }
        }

        public Dictionary<int, IWeapon> GetAvailableWeapons()
        {
            return weapons;
        }

        public void RegisterWeapon(IWeapon weapon, int slot)
        {
            weapons[slot] = weapon;

            OnWeaponRegistered?.Invoke(slot);
            next = Math.Max(next, slot + 1);
        }

        public int RegisterWeaponNext(IWeapon weapon)
        {
            int oldNext = next;
            RegisterWeapon(weapon, next);
            return oldNext;
        }

        public void SetTargetForAll(GameObject target)
        {
            foreach (var item in weapons)
            {
                weapons[item.Key].SetTarget(target);
            }
        }

        public void StopAllWeapons()
        {
            foreach (var item in weapons)
            {
                weapons[item.Key].StopWeapon();
            }
        }

        public void StopWeapon(int slot)
        {
            if (CheckWeaponSlot(slot))
            {
                weapons[slot].StopWeapon();
            }
            else
            {
                Debug.LogWarning($"No weapon registered at slot {slot}");
            }
        }

        public void UnregisterWeapon(int slot)
        {
            if (CheckWeaponSlot(slot))
            {
                weapons.Remove(slot);
                OnWeaponUnregistered?.Invoke(slot);
            }
            else
            {
                Debug.LogWarning($"No weapon registered at slot {slot}");
            }
        }
        private bool CheckWeaponSlot(int slot)
        {
            return weapons.ContainsKey(slot);
        }

        public void ReloadAllWeapons()
        {
            foreach (var item in weapons)
            {
                weapons[item.Key].Reload();
            }
        }
    }



}