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


        public event Action<int> OnAmmoChange;
        public event Action OnWeaponPrep;
        //Bool is whether the weapon was successfully fired
        public event Action<bool> OnWeaponFire;
        public event Action OnReload;
    }
}
