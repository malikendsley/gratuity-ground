using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;

namespace Endsley
{
    public class MechWeaponManager : MonoBehaviour, IWeaponManager
    {

        readonly Dictionary<int, IWeapon> weapons = new();
        readonly Dictionary<int, bool> weaponFireState = new();
        readonly Dictionary<int, bool> weaponPrepState = new();
        readonly Dictionary<int, bool> weaponAimAssistState = new();
        readonly Dictionary<int, GameObject> weaponTargetState = new();
        public event Action<int> OnWeaponRegistered;
        public event Action<int> OnWeaponUnregistered;
        //ID of the next weapon if the weaponManager is allowed to handle it
        private int next = 1;
        // Update is called once per frame
        void Update()
        {
            //Read the fire and prep state of all weapons and call their methods
            foreach (var item in weapons)
            {
                int slot = item.Key;

                // Check if the weapon should fire
                if (weaponFireState[slot])
                {
                    // Check if aim assist should be applied
                    bool shouldAimAssist = weaponAimAssistState.TryGetValue(slot, out bool assist) && assist;
                    weapons[slot].FireWeapon(shouldAimAssist);  // Updated this line
                }

                // Check if the weapon should be prepped
                if (weaponPrepState[slot])
                {
                    weapons[slot].PrepWeapon();
                }

                // Check if the weapon has a target
                if (weaponTargetState.TryGetValue(slot, out GameObject target))
                {
                    weapons[slot].SetTarget(target);
                }
            }
        }


        public void FireAllWeapons()
        {
            foreach (var item in weapons)
            {
                weaponFireState[item.Key] = true;
            }
        }

        public void FireWeapon(int slot, bool shouldAimAssist = false)
        {
            Debug.Log("Firing weapon in slot " + slot);
            if (CheckWeaponSlot(slot))
            {
                weaponFireState[slot] = true;
                weapons[slot].FireWeapon(shouldAimAssist);
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

        public void PrepAllWeapons()
        {
            foreach (var item in weapons)
            {
                weaponPrepState[item.Key] = true;
            }
        }

        // Similar checks for other public methods interacting with weapon slots
        public void PrepWeapon(int slot)
        {
            if (CheckWeaponSlot(slot))
            {
                weaponPrepState[slot] = true;
            }
            else
            {
                Debug.LogWarning($"No weapon registered at slot {slot}");
            }
        }

        public void RegisterWeapon(IWeapon weapon, int slot)
        {
            weapons[slot] = weapon;
            weaponFireState[slot] = false;
            weaponPrepState[slot] = false;

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
                weaponTargetState[item.Key] = target;
            }
        }

        public void StopAllWeapons()
        {
            foreach (var item in weapons)
            {
                weaponFireState[item.Key] = false;
            }
        }

        public void StopWeapon(int slot)
        {
            if (CheckWeaponSlot(slot))
            {
                weaponFireState[slot] = false;
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
    }



}