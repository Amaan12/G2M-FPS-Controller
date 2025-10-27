using UnityEngine;

public class GravityNade : Nade
{
    [SerializeField] GameObject gravityTriggerZone;

    protected override void Detonate()
    {
        // play explosion PFX and SFX at transform.position
        Instantiate(pfx, transform.position, pfx.transform.rotation);
        Instantiate(gravityTriggerZone, transform.position, Quaternion.identity);

        // Apply anti-gravity effect within a XZ radius of the transform.position

        Destroy(gameObject);
    }
}
