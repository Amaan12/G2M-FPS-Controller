using UnityEngine;

public class HealthController : MonoBehaviour
{
    public float maxHealth = 100f;
    public float health = 100f;
    Enemy enemy;

    public ObjectPool<Enemy> enemyPool;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void AssignPool(ObjectPool<Enemy> pool)
    {
        enemyPool = pool;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (enemyPool != null)
        {
            enemyPool.Return(enemy);
        }
        else
        {
            Debug.LogWarning("enemyPool is not assigned in HealthController.");
        }
    }
}
