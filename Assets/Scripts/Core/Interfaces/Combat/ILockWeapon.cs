using System;
using UnityEngine;
namespace Endsley
{

    // Lock weapons need to emit an OnLockTick event every time they acquire a new lock
    // UI and sound can link to this event to update the player
    public interface ILockWeapon : IWeapon
    {
        public event Action<GameObject> OnLockStack;
    }

}