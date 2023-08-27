using System;
using UnityEngine;

public class DieBehavior : MonoBehaviour, IDie
{

    public event Action OnDie;

    public void Die()
    {
        OnDie?.Invoke();
        Debug.Log("I'm dying.");
        // Additional death logic
    }
}