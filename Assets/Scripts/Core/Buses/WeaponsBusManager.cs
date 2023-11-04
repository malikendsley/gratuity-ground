using System;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public enum WeaponEventType
    {
        OnAmmoDecrease,
        OnWeaponStart,
        OnWeaponStop,
        OnReloadStart,
        OnReloadFinish,
        OnLockStack,
        OnTargetChange,
        OnTargetClear,
    }

    public class WeaponEventData
    {
        public GameObject Target { get; set; }
        public IWeapon Weapon { get; set; }
        public int RemainingAmmo { get; set; }
    }
    public class WeaponsBusManager : MonoBehaviour
    {
        public static WeaponsBusManager Instance;
        // HACK: remove this and its references
        public Dictionary<GameObject, WeaponsBus> mechToBusMap = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public WeaponsBus GetOrCreateBus(GameObject mech)
        {
            if (mechToBusMap.TryGetValue(mech, out WeaponsBus bus))
            {
                return bus;
            }

            bus = new WeaponsBus();
            mechToBusMap[mech] = bus;
            return bus;
        }
    }

    public class WeaponsBus : IBus<WeaponEventType, WeaponEventData>
    {
        public bool debug = false;

        private readonly Dictionary<WeaponEventType, List<Action<WeaponEventData>>> eventHandlers = new();

        public void Subscribe(WeaponEventType eventType, Action<WeaponEventData> handler)
        {
            if (debug) { Debug.Log("Subscribing to " + eventType); }
            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Action<WeaponEventData>>();
            }

            if (!eventHandlers[eventType].Contains(handler))  // Safety Check
            {
                eventHandlers[eventType].Add(handler);
            }
            else
            {
                Debug.LogWarning("Handler is already subscribed to " + eventType);
            }
        }

        public void Unsubscribe(WeaponEventType eventType, Action<WeaponEventData> handler)
        {
            if (debug) { Debug.Log("Unsubscribing from " + eventType); }
            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Remove(handler);
            }
        }

        public void Emit(WeaponEventType eventType, WeaponEventData eventData)
        {
            if (debug) { Debug.Log("Emitting " + eventType); }
            if (eventHandlers.ContainsKey(eventType))
            {
                foreach (var handler in eventHandlers[eventType])
                {
                    handler.Invoke(eventData);
                }
            }
        }
    }

}
