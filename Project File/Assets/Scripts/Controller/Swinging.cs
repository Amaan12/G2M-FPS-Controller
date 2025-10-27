using UnityEngine;

public class Swinging : MonoBehaviour {

    [Header("References")]
    public LineRenderer lr;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, cam, player;
    public PlayerMovement pm;

    [Header("Swing settings")]
    private float maxSwingDistance = 100f;
    private Vector3 swingPoint;
    private SpringJoint joint;
    public float horizontalThrustForce;
    public float forwardThrustForce;
    public float extendCableSpeed;
    public int MouseCode;
    public KeyCode key;

    public float spring;
    public float damper;
    public float massScale;

    public bool swinging;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius;
    public Transform predictionPoint;


    void Update() {
        if (Input.GetKeyDown(key)) StartSwing();
        else if (Input.GetKeyUp(key)) StopSwing();

        CheckForSwingPoints();

        if (IsGrappling()) OdmGearMovement();
        else StopSwing();
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartSwing() {
        swinging = true;
        // RaycastHit hit;
        // if (Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable)) {
        //     swingPoint = hit.point;
        //     joint = player.gameObject.AddComponent<SpringJoint>();
        //     joint.autoConfigureConnectedAnchor = false;
        //     joint.connectedAnchor = swingPoint;

        //     float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //     //The distance grapple will try to keep from grapple point. 
        //     joint.maxDistance = distanceFromPoint * 0.8f;
        //     joint.minDistance = distanceFromPoint * 0.25f;

        //     //Adjust these values to fit your game.
        //     joint.spring = 4.5f;
        //     joint.damper = 7f;
        //     joint.massScale = 4.5f;

        //     lr.positionCount = 2;
        //     currentGrapplePosition = gunTip.position;
        // }

        // return if predictionHit not found
        if (predictionHit.point == Vector3.zero) return;

        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        //The distance grapple will try to keep from grapple point. 
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        //Adjust these values to fit your game.
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }


    void StopSwing() {
        swinging = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        // currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 2f);
        
        lr.SetPosition(0, gunTip.position);
        // lr.SetPosition(1, currentGrapplePosition);
        lr.SetPosition(1, swingPoint);
    }

    public bool IsGrappling() { return joint != null; }
    public Vector3 GetGrapplePoint() { return swingPoint; }

    void CheckForSwingPoints()
    {
        if (IsGrappling()) return;
        RaycastHit sphereCastHit, raycastHit;
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxSwingDistance, whatIsGrappleable);
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingDistance, whatIsGrappleable);
        Vector3 realHitPoint;
        if (raycastHit.point != Vector3.zero) realHitPoint = raycastHit.point;
        else if (sphereCastHit.point != Vector3.zero) realHitPoint = sphereCastHit.point;
        else realHitPoint = Vector3.zero;

        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    // -----------------------------------------------------------------------------------------------------

    private void OdmGearMovement()
    {
        // right
        // if (Input.GetKey(KeyCode.D)) pm.rb.AddForce(pm.orientation.right * horizontalThrustForce * Time.deltaTime);
        // // left
        // if (Input.GetKey(KeyCode.A)) pm.rb.AddForce(-pm.orientation.right * horizontalThrustForce * Time.deltaTime);
        // // forward
        // if (Input.GetKey(KeyCode.W)) pm.rb.AddForce(pm.orientation.forward * forwardThrustForce * Time.deltaTime);
        // back
        // if (Input.GetKey(KeyCode.S)) pm.rb.AddForce(-pm.orientation.forward * forwardThrustForce * Time.deltaTime);

        // shorten cable
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            pm.rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);
            float distanceFromPoint = directionToPoint.magnitude;

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }

        // extend cable
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;
            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }
    }
}
