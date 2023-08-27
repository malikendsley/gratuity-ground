using System;

public interface IDie
{
    event Action OnDie;

    void Die();
}