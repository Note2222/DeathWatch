using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageableInterface
{
    float MaxHealth { get; }
    float CurrentHealth { get; }

    void TakeDamage(float damageAmount);
    void Heal(float healAmount);
}