using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public Enemy enemyPrefab;
    ObjectPool<Enemy> enemyPool;

    public Transform player;

    void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(enemyPrefab, 5);
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 2f, 3f);
    }

    void SpawnEnemy()
    {
        Enemy enemyInstance = enemyPool.Get();
        enemyInstance.GetComponent<EnemyHealthController>().AssignPool(enemyPool);
        enemyInstance.GetComponent<EnemyHealthController>().health = enemyInstance.GetComponent<EnemyHealthController>().maxHealth;
        enemyInstance.player = player;
        // Vector3 randomPosOnCollider = a world position randomly on the collider bounds
        Collider collider = GetComponent<Collider>();
        Vector3 randomPosOnCollider = collider != null
            ? collider.bounds.center + new Vector3(
            Random.Range(-collider.bounds.extents.x, collider.bounds.extents.x),
            Random.Range(-collider.bounds.extents.y, collider.bounds.extents.y),
            Random.Range(-collider.bounds.extents.z, collider.bounds.extents.z))
            : transform.position;
        enemyInstance.transform.SetPositionAndRotation(randomPosOnCollider, Quaternion.identity);

    }
}
