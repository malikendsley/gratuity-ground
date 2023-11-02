using System;
using Endsley;

public interface IDamageable
{
    event Action<int> OnDamageTaken;
    void TakeDamage(int damage);
    Allegiance GetAllegiance();
}