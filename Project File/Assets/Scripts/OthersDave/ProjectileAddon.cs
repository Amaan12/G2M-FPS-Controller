using UnityEngine;

public class ProjectileAddon : MonoBehaviour
{
    Rigidbody rb;
    bool targetHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (targetHit) return;
        else targetHit = true;
        
        rb.isKinematic = true; // Stop the projectile from moving
        transform.SetParent(other.transform);
    }
}
