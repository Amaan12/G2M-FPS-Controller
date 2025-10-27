using System;
using LevelManagement;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float health = 1000f;

    public static event Action<float> OnHealthChanged;

    void Start()
    {
        OnHealthChanged?.Invoke(health);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(health);
        if (health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        LevelLoader.ReloadLevel();
    }
}
