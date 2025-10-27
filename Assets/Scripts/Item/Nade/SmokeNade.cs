using UnityEngine;

public class SmokeNade : Nade
{
    protected override void Detonate()
    {
        // play explosion PFX and SFX at transform.position
        Instantiate(pfx, transform.position, pfx.transform.rotation);

        Destroy(gameObject);
    }
}
