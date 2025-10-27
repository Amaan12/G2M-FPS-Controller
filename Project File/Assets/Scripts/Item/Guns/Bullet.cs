using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    TrailRenderer myTrailRenderer;
    public ObjectPool<Bullet> bulletPool;
    [SerializeField] float speed = 15f;
    float timer = 5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myTrailRenderer = GetComponent<TrailRenderer>(); 

        rb.useGravity = false;
    }

    void Update()
    {
        // Destroy the bullet after a timer expires
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Return to pool instead of destroying
            bulletPool.Return(this);
        }
    }

    public void BulletShoot()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    public void AssignBulletPool(ObjectPool<Bullet> pool)
    {
        bulletPool = pool;
    }

    public void ResetBullet()
    {
        timer = 5f;
        myTrailRenderer.Clear();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy the bullet on collision
        if (bulletPool != null) bulletPool.Return(this);
        else Destroy(gameObject);
    }
}
