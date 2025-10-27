using UnityEngine;

public class HENade : Nade
{
    [SerializeField] float explosionForce = 1000f;

    protected override void Detonate()
    {
        // play explosion PFX and SFX at transform.position
        // Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(transform.position, nadeRadius);
        foreach (Collider hit in hits)
        {
            // AddExplosionForce to all rigidbodies within the explosion radius
            Rigidbody hitRb = hit.transform.GetComponentInParent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, nadeRadius);
                Debug.Log("Explosion force applied to: " + hit.name);
            }

            // Apply damage to all objects with a health controller within the explosion radius
            // Transform root = hit.transform.parent; // Assuming hit is a child (like a body part)
            // if (root != null)
            // {
            //     HealthController healthController = root.GetComponent<HealthController>();
            //     if (healthController != null)
            //     {
            //         healthController.TakeDamage(50f); // Example damage value
            //         Debug.Log("Damage applied to: " + root.name);
            //     }
            // }

            // Optional - Destructible scrpit on destructible objects
            // hit.GetComponent<Destructible>()?.DestroyObject();
            // Collider[] collidersToMove = Physics.OverlapSphere(transform.position, nadeRadius);
            // foreach (Collider collider in collidersToMove)
            // {
            //     Rigidbody hitRb2 = hit.GetComponent<Rigidbody>();
            //     if (hitRb2 != null)
            //     {
            //         hitRb2.AddExplosionForce(1000f, transform.position, nadeRadius);
            //         Debug.Log("Explosion force applied to: " + hit.name);
            //     }
            // }

            // Invoke explosion effect on any barrels within the radius, i.e. call explode() on them
        }

        Destroy(gameObject);
    }
}
