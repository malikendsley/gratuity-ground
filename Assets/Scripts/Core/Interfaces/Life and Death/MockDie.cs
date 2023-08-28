using System;
using UnityEngine;


namespace Endsley
{
    public class MockDie : MonoBehaviour, IDie
    {
        public event Action OnDie;
        public bool DieCalled { get; private set; } = false;
        public void Die()
        {
            DieCalled = true;
            OnDie?.Invoke();
        }
    }
}
