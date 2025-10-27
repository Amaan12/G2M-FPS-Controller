using UnityEngine;

public class MolotovNade : Nade
{
    [SerializeField] float lifeTime = 10f;
    [SerializeField] private LayerMask groundLayer;

    protected override void Throw()
    {
        thrown = true;
        transform.parent = null;
        rb.isKinematic = false;
        col.enabled = true;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        // Invoke(nameof(Detonate), fuseTime);
    }

    protected override void Detonate()
    {
        for (int i = 0; i < 16 * 3; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * nadeRadius;

            if (Physics.Raycast(transform.position + randomDirection, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                GameObject molotovPFXPrefab = Instantiate(pfx, hit.point, Quaternion.identity);
                // Destroy(molotovPFXPrefab, lifeTime); // destroy after lifeTime  
            }
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (thrown && collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Detonate(); // detonate on ground collision
        }
    }
}
