using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody rb;
    public Bullet bulletPrefab;

    private StateMachine<Enemy> stateMachine;
    private State<Enemy> enemyPatrolState;
    private State<Enemy> enemyAttackState;

    public Transform player;
    public Transform firePoint;

    public ObjectPool<Bullet> bulletPool;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletPool = new ObjectPool<Bullet>(bulletPrefab, 10);

        stateMachine = new StateMachine<Enemy>();
        enemyPatrolState = new EnemyPatrolState(this, rb);
        enemyAttackState = new EnemyAttackState(this);

        stateMachine.ChangeState(enemyPatrolState);
    }

    public void Update()
    {
        stateMachine?.Update();
    }

    public void OnPlayerFound()
    {
        stateMachine.ChangeState(enemyAttackState);
    }

    public void OnPlayerLeft()
    {
        stateMachine.ChangeState(enemyPatrolState);
    }

    public void Align(Transform target)
    {
        // Rotate towards the target on y axis and shoot
        Vector3 direction = (target.position - firePoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        firePoint.rotation = Quaternion.Lerp(firePoint.rotation, lookRotation, Time.deltaTime / 0.2f);
    }

    public void Shoot()
    {
        // Simulate shooting (replace with your shooting logic)
        Bullet bulletInstance = bulletPool.Get();
        bulletInstance.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        if (bulletInstance.bulletPool == null) bulletInstance.AssignBulletPool(bulletPool);
        bulletInstance.BulletShoot();
        bulletInstance.ResetBullet();
    }
}

public class EnemyPatrolState : State<Enemy>
{
    Rigidbody rb;
    private Vector3 wanderTarget;

    public EnemyPatrolState(Enemy owner, Rigidbody rb) : base(owner)
    {
        this.rb = rb;
    }

    public override void Enter()
    {
        wanderTarget = owner.transform.forward * 5f;
    }

    public override void Update()
    {
        // Move randomly around the area using Reynold's wander behavior
        float wanderRadius = 2f;
        float wanderDistance = 5f;
        float wanderJitter = 0.2f;

        // Store wander target as a field for continuity
        if (wanderTarget == Vector3.zero)
        {
            wanderTarget = rb.transform.forward * wanderDistance;
        }

        // Add small random vector to the wander target
        wanderTarget += new Vector3(
            Random.Range(-1f, 1f) * wanderJitter,
            0,
            Random.Range(-1f, 1f) * wanderJitter
        );

        wanderTarget = wanderTarget.normalized * wanderRadius;

        // Calculate target in world space
        Vector3 targetLocal = wanderTarget + Vector3.forward * wanderDistance;
        Vector3 targetWorld = rb.transform.TransformPoint(targetLocal);

        // Move towards the target
        Vector3 direction = (targetWorld - rb.transform.position).normalized;
        float speed = 15f;
        rb.MovePosition(rb.transform.position + direction * speed * Time.deltaTime);

        // Corrected logic: If player is within detection range, switch to attack state
        float detectionRange = 10f;
        float distanceToPlayer = Vector3.Distance(owner.transform.position, owner.player.position);
        if (distanceToPlayer < detectionRange)
        {
            owner.OnPlayerFound();
        }
    }

    public override void Exit()
    {
        // Code to execute when exiting patrol state
    }
}

public class EnemyAttackState : State<Enemy>
{
    float fireRate = 4f; // Shoot every half second
    float nextFireTime = 0f;

    public EnemyAttackState(Enemy owner) : base(owner) { }

    public override void Enter()
    {
        // Code to execute when entering patrol state
    }

    public override void Update()
    {
        // Shoot the player
        owner.Align(owner.player);
        if (Time.time >= nextFireTime)
        {
            owner.Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
        
        // If outside the detection range return to patrol state
        float detectionRange = 10f;
        float distanceToPlayer = Vector3.Distance(owner.transform.position, owner.player.position);
        if (distanceToPlayer > detectionRange)
        {
            owner.OnPlayerLeft();
        }
    }

    public override void Exit()
    {
        // Code to execute when exiting patrol state
    }
}