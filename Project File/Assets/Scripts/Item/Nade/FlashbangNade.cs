using UnityEngine;

public class FlashbangNade : Nade
{
    [SerializeField] float maxFlashAngle = 90f; 
    [SerializeField] float flashDuration = 5f;

    protected override void Detonate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, nadeRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Transform playerRoot = hit.transform.parent; // Assuming hit is a child (like a body part)
                if (playerRoot == null) continue;

                // map flashDuration, inversely distance between player and transform from 0 to flashRadius, 
                // so that flashDuration is max at 0 distance and 0 at flashRadius
                float distance = Vector3.Distance(transform.position, playerRoot.position);
                flashDuration = Mathf.Lerp(flashDuration, 0f, distance / nadeRadius);

                Transform cameraPoint = playerRoot.parent?.Find("CameraPoint");
                if (cameraPoint == null) continue;

                Vector3 toFlash = (transform.position - Camera.main.transform.position).normalized;
                float angle = Vector3.Angle(toFlash, Camera.main.transform.forward); // direction to flash vs where player looks
                // Are you sure cameraPoint.forward is correct?

                if (angle < maxFlashAngle)
                {
                    PlayerFlashHandler handler = hit.GetComponentInParent<PlayerFlashHandler>();
                    if (handler != null)
                    {
                        handler.ApplyFlash(flashDuration);
                    }
                    else Debug.LogWarning("NO PlayerFlashHandler found for " + hit.name);
                }
            }
        }

        Destroy(gameObject);
    }
}
