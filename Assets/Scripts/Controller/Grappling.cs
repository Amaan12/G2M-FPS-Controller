using UnityEngine;

public class Grappling : MonoBehaviour
{
    PlayerMovement pm;
    [SerializeField] Transform gunTip;
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] LineRenderer lr;

    // Grapple settings
    [SerializeField] float maxGrappleDistance;
    [SerializeField] float grapplyDelayTime;
    Vector3 grapplePoint;
    public bool grappling;
    [SerializeField] float overshootYAxis;

    // Cooldown
    [SerializeField] float grappleCooldown;
    float grapplingCooldownTimer;

    void Awake()
    {
        pm = GetComponent<PlayerMovement>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) StartGrapple();
        if (grapplingCooldownTimer > 0) grapplingCooldownTimer -= Time.deltaTime;
    }

    void LateUpdate()
    {
        if (grappling) lr.SetPosition(0, gunTip.position);
    }
    
    void StartGrapple()
    {
        if (grapplingCooldownTimer > 0) return;
        grappling = true;
        // pm.freeze = true;
        RaycastHit hit;
        if (Physics.Raycast(pm.playerCam.transform.position, pm.playerCam.transform.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grapplyDelayTime);
        }
        else
        {
            grapplePoint = pm.playerCam.transform.position + pm.playerCam.transform.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grapplyDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    void ExecuteGrapple()
    {
        // pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    void StopGrapple()
    {
        grappling = false;
        // pm.freeze = false;
        lr.enabled = false;
        grapplingCooldownTimer = grappleCooldown;
    }

    Vector3 velocityToSet;
    void SetVelocity()
    {
        pm.rb.linearVelocity = velocityToSet;
    }

    void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    // Helper function (Get velocity required to reach the grapple point, a projectile equation question)
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endpoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endpoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endpoint.x - startPoint.x, 0, endpoint.z - startPoint.z);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(Mathf.Abs(2 * (displacementY - trajectoryHeight) / gravity)));
        return velocityXZ + velocityY;
    }
}















