using System;

public interface IDamageable
{
    event Action<int> OnDamageTaken;
    void TakeDamage(int damage);
}