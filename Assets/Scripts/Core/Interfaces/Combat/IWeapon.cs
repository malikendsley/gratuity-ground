using System;
using UnityEngine;
namespace Endsley
{
    public interface IWeapon
    {
        public void StartWeapon(bool isPerfectShot);
        public void StopWeapon();
        public void Reload();

        public void SetTarget(GameObject target);

        // Int for the desired weapon slot for player weapons. 
        // -1 for no slot (take next available)
        int AssignedSlot { get; set; }

        public event Action<int> OnAmmoChange;
        public event Action OnWeaponStart;
        //Bool is whether the weapon was successfully fired
        public event Action OnWeaponStop;
        public event Action OnReload;
    }
}
