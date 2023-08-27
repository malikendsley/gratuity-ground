using System;
using UnityEngine;

public interface IDamageable
{
    event Action<int> OnDamageTaken;
    void TakeDamage(int damage);
}