using UnityEngine;
using System.Collections;

public class ItemHolderPickup : MonoBehaviour
{
    public GameObject currentItem;    // Currently held item
    private Rigidbody rb;

    public float pickupRange = 3f;     // How far the player can pick up items
    public Coroutine lerpRoutine;
    public float lerpDuration = 0.25f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (currentItem == null) TryPickup();
            else DropItem();
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickup")) // Make sure your pickable objects have this tag
            {
                currentItem = hit.collider.gameObject;

                // // Parent the item to the itemHolder
                // currentItem.transform.localPosition = Vector3.zero;
                // currentItem.transform.localRotation = Quaternion.identity;

                // Disable physics while held
                rb = currentItem.GetComponentInParent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;
                hit.collider.enabled = false;

                currentItem.transform.SetParent(transform);
                // Start smooth Lerp to holder position
                if (lerpRoutine != null) StopCoroutine(lerpRoutine);
                lerpRoutine = StartCoroutine(LerpToHolder(currentItem.transform));

            }
        }
    }

    IEnumerator LerpToHolder(Transform item)
    {
        Vector3 startPos = item.localPosition;
        Quaternion startRot = item.localRotation;

        float elapsed = 0f;
        while (elapsed < lerpDuration)
        {
            if (item == null) yield break; // Item dropped early

            item.localPosition = Vector3.Lerp(startPos, Vector3.zero, elapsed / lerpDuration);
            item.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, elapsed / lerpDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;

        if (currentItem.TryGetComponent<Gun>(out Gun gun))
        {
            gun.enabled = true;
        }
    }

    void DropItem()
    {
        if (currentItem == null) return;

        if (lerpRoutine != null) StopCoroutine(lerpRoutine);
        // Unparent
        currentItem.transform.SetParent(null);

        if (currentItem.TryGetComponent<Gun>(out Gun gun))
        {
            gun.enabled = false;
        }

        // Enable physics
        // rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        currentItem.GetComponent<Collider>().enabled = true;

        // Optionally add a small forward force on drop
        rb?.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);
        rb = null;
        currentItem = null;
    }
}
