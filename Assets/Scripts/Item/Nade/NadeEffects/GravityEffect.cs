using UnityEngine;

public class GravityEffect : MonoBehaviour
{
    [SerializeField] float upwardForce = 30f;

    void Start()
    {
        Destroy(gameObject, 15f);
    }

    void Update()
    {
    }

    void OnTriggerStay(Collider other)
    {
        other.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
    }
}
