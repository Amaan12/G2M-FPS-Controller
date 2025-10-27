using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Transform orientation;
    public Rigidbody rb;
    float moveSpeed = 5f;
    public float walkSpeed, sprintSpeed, slideSpeed, wallRunSpeed, dashSpeed, swingSpeed;
    public float terminalDrag = 20f;
    public float maxYSpeed;
    float horizontalInput, verticalInput;
    Vector3 moveDirection;

    float desiredMoveSpeed, lastDesiredMoveSpeed;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public float groundDrag, airDrag;

    [Header("Air Control")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    public float startYScale;
    public bool stoppedCrouching, startedCrouching;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce, downhillMultiplier, speedIncreaseMultiplier, slopeIncreaseMultiplier;
    float slideTimer;
    public float slideYScale;
    // float startYScale;
    public bool sliding;

    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public float wallRunForce;
    public float maxWallRunTime;
    float wallRunTimer;
    bool wallrunning;

    bool upwardsRunning, downwardsRunning;
    public float wallClimbSpeed;

    [Header("Wall Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    RaycastHit leftWallHit, rightWallHit;
    bool wallLeft, wallRight;

    [Header("Wall Jumping")]
    public float wallJumpUpForce;
    public float wallJumpSideForce;

    [Header("Exiting Wall State")]
    bool exitingWall;
    public float exitWallTime;
    float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Camera")]
    public PlayerCam playerCam;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    float climbTimer;
    bool climbing;

    [Header("Climbing Wall Detection")]
    public float detectionLength;
    public float spherecastRadius;
    public float maxWallLookAngle;
    float wallLookAngle;
    RaycastHit frontWallHit;
    bool wallFront;

    [Header("Climb Jumping")]
    public float climbJumpUpForce;
    public float climbJumpBackForce;
    public int climbJumps;
    int climbJumpsLeft;

    [Header("Climb Jumping Detection")]
    Transform lastWall;
    Vector3 lastWallNormal;
    public float minWallNormalAngle;

    [Header("CLimb Jump Exit")]
    // public bool exitingClimbingWall;
    // used the same exiting wall variables of wall running for climbing, so no need to create new ones.

    [Header("Ledge Detection")]
    public float ledgeDetectionLength;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;
    Transform lastLedge, currentLedge;
    RaycastHit ledgeHit;

    [Header("Ledge Grabbing")]
    public float moveToLedgeSpeed;
    public float maxLedgeGrabDistance;
    public float minTimeOnLedge;
    float timeOnLedge;
    public bool holding;

    [Header("Extra States")]
    public bool freeze, unlimited, restricted;
    public Grappling grapplingScript;
    public bool swinging;

    [Header("Ledge Jumping")]
    public float ledgeJumpForwardForce;
    public float ledgeJumpUpwardForce;

    [Header("Exiting Ledge")]
    public bool exitingLedge;
    public float exitLedgeTime;
    float exitLedgeTimer;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public bool dashing;
    public float maxDashYSpeed;
    public float fovWhileDashingMultiplier;

    // public float dashSpeedChangeFactor;

    [Header("DashCooldown")]
    public float dashCooldown;
    float dashCooldownTimer;

    [Header("Dash Settings for omni-directional dashing")]
    public bool useCameraForward = true;
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    public MovementState state;
    public enum MovementState
    {
        freeze,
        unlimited,
        walking,
        sprinting,
        air,
        crouching,
        sliding,
        wallrunning,
        climbing,
        dashing,
        swinging
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
    }

    void Update()
    {
        MyInput();
        SetDrag();
        // SpeedControl(); // Commented out cuz ever since Sliding, its causing bugs becuase player enters walk or air state and the player stops.
        StateHandler();
        // Debug.Log(rb.linearVelocity.magnitude);

        // Sliding
        if (Input.GetKeyDown(KeyCode.LeftControl) && (horizontalInput != 0 || verticalInput != 0)) StartSlide();
        if (Input.GetKeyUp(KeyCode.LeftControl) && sliding) StopSlide();

        // Wall Running/Jumping
        CheckForWall();
        StateMachine();

        // Climbing.
        WallCheck();
        ClimbingStateMachine();

        // Ledge Climbing
        LedgeDetection();
        LedgeStateMachine();

        // Dashing
        if (Input.GetKeyDown(KeyCode.E)) Dash();
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        MovePlayer();
        if (sliding) SlidingMovement();
        if (wallrunning) WallRunningMovement();
        if (climbing && !exitingWall) ClimbingMovement();
    }

    void SetDrag()
    {
        grounded = Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0.5f, 0f), Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        // no drag when dashing.
        // if (activeGrapple) rb.linearDamping = 0f;

        // if (state == MovementState.swinging)  rb.linearDamping = airDrag / 2;
        // if (grapplingScript.grappling) rb.linearDamping = 0f;
        if (grounded && (state != MovementState.dashing)) rb.linearDamping = groundDrag;
        else rb.linearDamping = airDrag;
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Vector3 targetScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
            
            // next line unrequired, as my pivot is at the bottom of the player
            // rb.position += new Vector3(0f, (crouchYScale - startYScale) * 0.5f, 0f); // move player down
        }
        else
        {
            Vector3 targetScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 10f);
            // rb.position += new Vector3(0f, (startYScale - crouchYScale) * 0.5f, 0f); // move player up
        }
    }

    void MovePlayer()
    {
        // if (activeGrapple) return;
        // if (swinging) return;
        if (restricted) return;
        if (exitingWall) return;
        if (state == MovementState.dashing) return;

        // calculate movement direction
        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 10f, ForceMode.Force);
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // stop player from going up when on slope
        }
        else if (grounded) rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            // Debug.Log("Air Control: " + moveDirection * moveSpeed * 10f * airMultiplier);
        }

        if (!wallrunning) rb.useGravity = !OnSlope(); // disable gravity when on slope
    }

    void SpeedControl()
    {
        // if (activeGrapple) return;
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                Vector3 desiredVel = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
                rb.linearVelocity = desiredVel;
            }
        }

        // limit Y velocity
        if (maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);
        }
    }

    #region Jumping

    void Jump()
    {
        exitingSlope = true;
        // Debug.Log($"Before Jump: {rb.linearVelocity}");
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        // Debug.Log($"After Jump: {rb.linearVelocity}");
    }   

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    #endregion

    #region Slope Handler AND State Handler

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0.5f, 0f), Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if (angle < maxSlopeAngle && angle != 0) return true;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    bool keepMomentum;
    MovementState lastState;

    void StateHandler()
    {
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0f;
            rb.linearVelocity = Vector3.zero;
        }
        else if (unlimited)
        {
            state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        }
        else if (climbing)
        {
            state = MovementState.climbing;
            desiredMoveSpeed = climbSpeed;
        }
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;
        }
        else if (swinging)
        {
            state = MovementState.swinging;
            desiredMoveSpeed = swingSpeed;
        }
        else if (!grounded)
        {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed; // Cuz we have air multiplier instead.
        }
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
        }
        else if (sliding)
        {
            state = MovementState.sliding;
            if (OnSlope() && rb.linearVelocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }
            else desiredMoveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothyLerpMoveSpeed2());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        // deactivate momentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
        

        // check if desiredMoveSpeed has changed drastically.
        // if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        // {
        //     StopAllCoroutines();
        //     StartCoroutine(SmoothyLerpMoveSpeed());
        // }
        // else
        // {
        //     moveSpeed = desiredMoveSpeed;
        // }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    #endregion

    #region Sliding

    void StartSlide()
    {
        sliding = true;
        slideTimer = maxSlideTime;
    }
    
    void SlidingMovement()
    {
        if (!grounded) return;
        if (rb.linearVelocity.magnitude < walkSpeed) return; // don't slide if not moving

        if (!OnSlope() || rb.linearVelocity.y > -0.1f) // normal sliding
        {
            rb.AddForce(moveDirection * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else // sliding downhill
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * slideForce * downhillMultiplier, ForceMode.Force);
        }
        if (slideTimer <= 0) StopSlide();
    }

    void StopSlide()
    {
        sliding = false;
    }

    IEnumerator SmoothyLerpMoveSpeed()
    {
        //Smoothly lerp movementSpeed to desired value
        float time = 0f;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        keepMomentum = false;
    }

    // more speed with steeper slopes
    IEnumerator SmoothyLerpMoveSpeed2()
    {
        //Smoothly lerp movementSpeed to desired value
        float time = 0f;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + slopeAngle / 90f; // 0.5f to 1.5f
                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }
    #endregion

    #region WallRunning

    void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0.5f, 0f), orientation.right, out rightWallHit, wallCheckDistance, whatIsWall) 
        || Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0f, 0f), orientation.right, out rightWallHit, wallCheckDistance, whatIsWall)
        || Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 1f, 0f), orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0.5f, 0f), -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall)
        || Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0f, 0f), -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall)
        || Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 1f, 0f), -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    bool AboveGround()
    {
        return !Physics.Raycast(transform.position + new Vector3(0f, playerHeight * 0.5f, 0f), Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, whatIsGround);
    }

    bool StateMachine()
    {
        // State 1 - Wall Running
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!wallrunning) StartWallRun();

            // if (wallRunTimer > 0) wallRunTimer -= Time.deltaTime;
            // if (wallRunTimer <= 0 && wallrunning)
            // {
            //     exitingWall = true;
            //     exitWallTimer = exitWallTime;
            // }
            
            // Wall Jump
            if (Input.GetKeyDown(KeyCode.Space)) WallJump();
        }

        // State 2 - Wall Running Exit
        else if (exitingWall)
        {
            if (wallrunning) StopWallRun();
            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0) exitingWall = false;
        }

        // State 3 - None
        else
        {
            if (wallrunning) StopWallRun();
        }
        return false;
    }

    void StartWallRun()
    {
        wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y velocity

        playerCam.DoFOV(90f);
        if (wallLeft) playerCam.DoTilt(-5f);
        if (wallRight) playerCam.DoTilt(5f);
    }

    void WallRunningMovement()
    {
        rb.useGravity = useGravity;
        if (useGravity) rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force); // counter the gravity

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude) wallForward = -wallForward; // flip the wall forward direction

        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force); // add force to the player in the direction of the wall
        
        // running upwards and downwards
        if (Input.GetKey(KeyCode.LeftShift)) rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallClimbSpeed, rb.linearVelocity.z);
        if (Input.GetKey(KeyCode.LeftControl)) rb.linearVelocity = new Vector3(rb.linearVelocity.x, -wallClimbSpeed, rb.linearVelocity.z);

        // push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    void StopWallRun()
    {
        wallrunning = false;

        playerCam.DoFOV(playerCam.startFOV);
        playerCam.DoTilt(0f);
    }

    #endregion

    #region WallJumping
    
    void WallJump()
    {
        if (holding || exitingLedge) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y velocity, to make jumping look clean.
        // rb.linearVelocity = new Vector3(0f, 0f, 0f); // reset full velocity?
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

    #endregion

    #region Climbing

    void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, spherecastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = (frontWallHit.transform != lastWall) || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngle;
        
        if ((wallFront && newWall) || grounded)
        {
            climbTimer = maxClimbTime; // reset climb timer when grounded
            climbJumpsLeft = climbJumps; // reset climb jumps when grounded
        }
    }

    void ClimbingStateMachine()
    {
        // State 0 - Ledge Grabbing
        if (holding)
        {
            if (climbing) StopClimbing();
            // you will lose climb time, even though you can't move, so just stop climbing.
        }

        // State 1 - Climbing
        else if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall && climbJumpsLeft > 0)
        {
            if (!climbing && climbTimer > 0) StartClimb();
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer <= 0) StopClimbing();
        }
        // State 2 - Climbing Exit
        else if (exitingWall)
        {
            if (climbing) StopClimbing();
            if (exitWallTimer > 0) exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0) exitingWall = false;
        }
        
        // State 3 - None
        else
        {
            if (climbing) StopClimbing();
        }


        if (wallFront && Input.GetKeyDown(KeyCode.Space) && wallLookAngle < maxWallLookAngle && climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    void StartClimb()
    {
        climbing = true;
        // camera fov change

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }

    void ClimbingMovement()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);
        // sound effect
    }

    void StopClimbing()
    {
        climbing = false;
        // particle effect
    }

    #endregion

    #region Climb Jumping

    void ClimbJump()
    {
        if (grounded) return;
        if (holding || exitingLedge) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbJumpBackForce;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset y velocity, to make jumping look clean.
        rb.AddForce(forceToApply, ForceMode.Impulse);
        climbJumpsLeft--;
    }
    #endregion

    #region Ledge Climbing

    void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, playerCam.transform.forward, out ledgeHit, ledgeDetectionLength, whatIsLedge);
        if (!ledgeDetected) return;
        
        float distanceToLedge = Vector3.Distance(transform.position, ledgeHit.transform.position);
        if (ledgeHit.transform == lastLedge) return;
        if (distanceToLedge < maxLedgeGrabDistance && !holding) EnterLedgeHold();
    }

    void LedgeStateMachine()
    {
        bool anyInputKeyPressed = horizontalInput != 0 || verticalInput != 0;

        // State 1 - Ledge Hold
        if (holding)
        {
            FreezeRigidbodyOnLedge();
            timeOnLedge += Time.deltaTime;
            if (timeOnLedge > minTimeOnLedge && anyInputKeyPressed) ExitLedgeHold();

            if (Input.GetKeyDown(KeyCode.Space)) LedgeJump();
        }
        else if (exitingLedge)
        {
            if (exitLedgeTimer > 0) exitLedgeTimer -= Time.deltaTime;
            else
            {
                exitingLedge = false;
            }
        }
    }

    void EnterLedgeHold()
    {
        holding = true;
        unlimited = true;
        restricted = true;

        currentLedge = ledgeHit.transform;
        lastLedge = ledgeHit.transform;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

    }

    void FreezeRigidbodyOnLedge()
    {
        rb.useGravity = false;
        Vector3 directionToLedge = currentLedge.position - (transform.position);
        float distanceToLedge = directionToLedge.magnitude;

        // Move Player To Ledge
        if (distanceToLedge > 1f)
        {
            if (rb.linearVelocity.magnitude < moveToLedgeSpeed)
                rb.AddForce(directionToLedge.normalized * moveToLedgeSpeed * 1000f * Time.deltaTime);
        }
        
        // Hold onto Ledge
        else
        {
            if (!freeze) freeze = true;
            if (unlimited) unlimited = false;
        }

        if (distanceToLedge > maxLedgeGrabDistance) ExitLedgeHold();
    }

    void ExitLedgeHold()
    {
        exitingLedge = true;
        exitLedgeTimer = exitLedgeTime;

        holding = false;
        restricted = false;

        timeOnLedge = 0f;
        freeze = false;
        rb.useGravity = true;

        StopAllCoroutines();
        Invoke(nameof(ResetLastLedge), 1f); // Can't regrab same ledge.
    }

    void ResetLastLedge()
    {
        lastLedge = null;
    }
    #endregion

    #region Ledge Jumping

    void LedgeJump()
    {
        ExitLedgeHold();
        Invoke(nameof(DelayedJumpForce), 0.05f); // delay jump force to make it look better
    }
        
    void DelayedJumpForce()
    {
        Vector3 forceToAdd = playerCam.transform.forward * ledgeJumpForwardForce + orientation.up * ledgeJumpUpwardForce;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(forceToAdd, ForceMode.Impulse);
    }

    #endregion

    #region Dashing

    public void Dash()
    {
        if (dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;

        dashing = true;
        maxYSpeed = maxDashYSpeed;
        playerCam.DoFOV(playerCam.startFOV * fovWhileDashingMultiplier);

        Transform forwardT;
        if (useCameraForward) forwardT = playerCam.transform;
        else forwardT = orientation;
        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;
        // Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;
        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);
        Invoke(nameof(ResetDash), dashDuration);

        if (disableGravity) rb.useGravity = false;
    }

    Vector3 delayedForceToApply;
    void DelayedDashForce()
    {
        if (resetVel) rb.linearVelocity = Vector3.zero;
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    public void ResetDash()
    {
        dashing = false;
        maxYSpeed = 0f;
        playerCam.DoFOV(playerCam.startFOV);
        rb.useGravity = true;
    }

    Vector3 GetDirection(Transform forwardT)
    {
        Vector3 direction = new Vector3();
        if (allowAllDirections) direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else direction = forwardT.forward;

        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }

    #endregion
}

