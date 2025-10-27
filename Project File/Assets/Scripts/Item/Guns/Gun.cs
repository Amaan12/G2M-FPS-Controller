using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] float damage = 25f;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] int bulletsPerShot = 1; // For shotgun, set >1
    [SerializeField] float spread = 0.05f;   // Spread angle in radians

    [Header("Effects")]
    [SerializeField] GameObject bulletImpactPFX;
    [SerializeField] GameObject bulletImpactDecal;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;

    [Header("Bullets")]
    float ammo;
    [SerializeField] float magSize;
    bool reloading;
    float shootTimer = 0;

    [Header("Reload")]
    [SerializeField] float reloadTime = 1.5f;
    [SerializeField] Animator animator;

    [Header("Sway")]
    [SerializeField] float swayAmount = 0.05f;
    [SerializeField] float swaySmooth = 8f;
    [SerializeField] float maxSway = 0.1f;
    public Vector3 initialLocalPos;

    [Header("Recoil Settings")]
    [SerializeField] float recoilUp = 2f;
    [SerializeField] float recoilSide = 1f;
    [SerializeField] float recoilRecoverySpeed = 5f;

    Camera cam;
    PlayerCam playerCam;
    ObjectPool<Bullet> bulletPool;

    void Awake()
    {
        cam = Camera.main;
        ammo = magSize;
        playerCam = cam.GetComponent<PlayerCam>();
        if (bulletPrefab != null)
            bulletPool = new ObjectPool<Bullet>(bulletPrefab.GetComponent<Bullet>(), 20);
    }

    void OnEnable()
    {
        initialLocalPos = transform.localPosition;
    }

    virtual protected void Update()
    {
        // Decrease timer
        if (shootTimer > 0)
            shootTimer -= Time.deltaTime;

        // Shooting input
        if (Input.GetMouseButton(0) && shootTimer <= 0 && !reloading && ammo > 0)
        {
            Shoot();
            ammo--;
            shootTimer = fireRate;
        }

        // Reload input (optional)
        if (Input.GetKeyDown(KeyCode.R) && !reloading && ammo < magSize)
            StartCoroutine(ReloadRoutine());

        Sway();
    }
    
    virtual protected void Shoot()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Vector3 shootDir = cam.transform.forward;
            // Apply spread
            shootDir = Quaternion.Euler(Random.Range(-spread, spread) * Mathf.Rad2Deg, Random.Range(-spread, spread) * Mathf.Rad2Deg, 0) * shootDir;

            if (Physics.Raycast(cam.transform.position, shootDir, out RaycastHit hit, range))
            {
                // Damage
                var health = hit.collider.GetComponent<HealthController>();
                if (health != null) health.TakeDamage(damage);

                // Impact FX
                if (bulletImpactPFX) Instantiate(bulletImpactPFX, hit.point, Quaternion.LookRotation(hit.normal));
                if (bulletImpactDecal) Instantiate(bulletImpactDecal, hit.point + hit.normal * 0.1f, Quaternion.LookRotation(hit.normal));
            }

            if (bulletPrefab != null)
            {
                Bullet bulletInstance = bulletPool.Get();
                bulletInstance.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
                if (bulletInstance.bulletPool == null) bulletInstance.AssignBulletPool(bulletPool);
                bulletInstance.BulletShoot();
                bulletInstance.ResetBullet();
            }

            playerCam.ApplyRecoil(recoilUp, recoilSide, recoilRecoverySpeed);
        }
    }

    IEnumerator ReloadRoutine()
    {
        reloading = true;
        if (animator) animator.SetTrigger("Reload");
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        ammo = magSize;
        reloading = false;
        Debug.Log("Reloaded!");
    }

    void Sway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Vector3 targetPos = initialLocalPos + new Vector3(-mouseX, -mouseY, 0) * swayAmount;
        targetPos.x = Mathf.Clamp(targetPos.x, initialLocalPos.x - maxSway, initialLocalPos.x + maxSway);
        targetPos.y = Mathf.Clamp(targetPos.y, initialLocalPos.y - maxSway, initialLocalPos.y + maxSway);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * swaySmooth);
    }
}
