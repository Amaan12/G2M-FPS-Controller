using UnityEngine;

public class Nade : MonoBehaviour
{
    [SerializeField] protected float throwForce = 30f;
    [SerializeField] protected float fuseTime = 3f;
    [SerializeField] protected float nadeRadius = 10f;

    protected Rigidbody rb;
    protected Collider col;
    protected bool thrown = false;

    [SerializeField] protected GameObject pfx;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        // rb.isKinematic = true; // assuming held state
    }

    void Update()
    {
        if (/*!thrown && */Input.GetMouseButtonDown(0))
        {
            if (transform.parent != null)
            {
                ItemHolderPickup itemHolder = transform.parent.GetComponent<ItemHolderPickup>();
                if (itemHolder != null && itemHolder.currentItem == gameObject)
                {
                    StopCoroutine(itemHolder.lerpRoutine);
                    Throw();
                }
            }
        }
    }

    protected virtual void Throw()
    {
        // thrown = true;
        transform.parent = null;
        rb.isKinematic = false;
        col.enabled = true;

        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        Invoke(nameof(Detonate), fuseTime);
    }

    protected virtual void Detonate()
    {
    }
}
