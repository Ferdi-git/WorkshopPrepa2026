using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector2 input;

    [Header("Movement")]
    public Rigidbody rb;
    public Transform cameraLookerTransform;
    public bool moving;
    public bool readyToRun;
    public bool unlockedSprint = false;
    public bool unlockedDash = false;
    public bool unlockedCrouch = false;
    public bool dashed = false;
    public bool canDash = false;


    public int numberOfAirJump;
    [SerializeField] private int currentNumberOfAirJump;

    [SerializeField] float currentSpeed;
    [SerializeField] float baseSpeed = 4;
    [SerializeField] float crouchspeed;
    [SerializeField] float acc;
    [SerializeField] float runForce;
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown =0.3f;
    [SerializeField] float dashDuration = 0.12f;
    [SerializeField] AnimationCurve dashCurve;

    Vector3 dashDirection; 
    float dashTimer;

    Vector2 smoothedInput;
    Vector2 inputVelocity;

    [Header("Jumping")]
    [SerializeField] float botRayHeight;
    [SerializeField] float botRaySize;
    [SerializeField] float botRayOffset;
    public float jumpDuration;
    [SerializeField] float coyoteTime = 0.12f;
    [SerializeField] float jumpBufferTime = 0.12f;

    [HideInInspector] public float coyoteTimer;
    [HideInInspector] public float jumpBufferTimer;

    private bool wasOnGround;

    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] AnimationCurve jumpDirectionCurve;

    [SerializeField] Transform CameraLooker;

    [SerializeField] LayerMask ignoredLayer;

    [HideInInspector]
    public float jumpTimer;
    [HideInInspector]
    public bool jumping;
    [HideInInspector]
    public bool touchingGround;

    Vector3 jumpDirection;
    Vector3 moveVector;

    [HideInInspector]
    public Vector3 vel;
    [HideInInspector]
    public bool running;
    public bool applyingRunForce;
    public bool isCrouching = false;
    private CapsuleCollider playerCollider;

    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        currentSpeed = baseSpeed;
        currentNumberOfAirJump = numberOfAirJump;
    }

    public void PlayerControlUpdate()
    {
        InputUpdate();

        GroundUpdate();

        JumpUpdate();
    }


    void InputUpdate()
    {
        moving = false;
        input = Vector2.zero;
        applyingRunForce = false;


        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            input.y += 1;

            if (!touchingGround)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && unlockedSprint)
                {
                    if (running) StopRunning();
                    else StartRunning();


                }

            }
            else
            {
                if(running)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                        StopRunning();
                   
                }
                else
                {

                    if (unlockedSprint && Input.GetKeyDown(KeyCode.LeftShift) || readyToRun )
                        StartRunning();
                    
                }
            }


            if(running)
            {
                applyingRunForce = true;
            }
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            input.y -= 1;
            if(running)
            {
                StopRunning();
            }
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            input.x -= 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            input.x += 1;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (!unlockedDash || !canDash) return;

            dashed = true;
            
        }

        if (Input.GetKey(KeyCode.CapsLock) && unlockedCrouch)
        {
            if (isCrouching == false) StartCrouching();
            isCrouching = true;
        }
        else
        {
            if (isCrouching == true) StopCrouching();
            isCrouching = false;
        }


        if (input.magnitude > 0) moving = true;
        else
        {
            if (running) StopRunning();

            if(Input.GetKey(KeyCode.LeftShift) && unlockedSprint)
            {
                readyToRun = true;
            }
            else
            {
                readyToRun = false;
            }
        }

        input = input.normalized;

        smoothedInput = Vector2.SmoothDamp(smoothedInput, input, ref inputVelocity, acc, 999f, Time.deltaTime);

    }

    void StartCrouching()
    {
        currentSpeed = crouchspeed;
        playerCollider.height = 1.5f;
        playerCollider.center = new Vector3(playerCollider.center.x , 0.75f , playerCollider.center.z);
        CameraLooker.localPosition = new Vector3(CameraLooker.localPosition.x, 0.5f, CameraLooker.localPosition.z);
    }

    void StopCrouching()
    {
        currentSpeed = baseSpeed;
        playerCollider.height = 2f;
        playerCollider.center = new Vector3(playerCollider.center.x, 1, playerCollider.center.z);
        CameraLooker.localPosition = new Vector3(CameraLooker.localPosition.x, 1.5f, CameraLooker.localPosition.z);
    }

    void StartRunning()
    { 
        readyToRun = false;
        running = true;

    }

    void StopRunning()
    {
        running = false;
    }

    public void MoveUpdate()
    {
        vel = Vector3.zero;

        Vector3 flatForward = cameraLookerTransform.forward;
        flatForward.y = 0;
        Vector3 flatRight = cameraLookerTransform.right;
        flatRight.y = 0;

        moveVector = (flatForward.normalized * smoothedInput.y) + (flatRight.normalized * smoothedInput.x);

        vel = moveVector * currentSpeed;

        if(applyingRunForce)
        {
            vel += new Vector3(cameraLookerTransform.forward.x, 0, cameraLookerTransform.forward.z).normalized * runForce;
        }

        if(jumping)
        {
            vel.y = jumpCurve.Evaluate(1f - (jumpTimer / jumpDuration));
 
            vel += jumpDirection * jumpDirectionCurve.Evaluate(1f - jumpTimer / jumpDuration);

            if (touchingGround)
            {
                if (vel.y < 0f) vel.y = 0f;
            }
        }
        else if(!touchingGround)
        {
            vel.y = jumpCurve.Evaluate(1f);
        }


        if (dashed)
        {
            dashed = false;
            dashTimer = dashDuration;

            dashDirection = new Vector3(cameraLookerTransform.forward.x, 0, cameraLookerTransform.forward.z).normalized;

            vel.y = 0f;
            StartCoroutine(DashCooldown());
        }

        if (dashTimer > 0f)
        {
            dashTimer = Mathf.Max(0f, dashTimer - Time.deltaTime);
            float t = 1f - (dashTimer / dashDuration);

            vel = dashDirection * (dashForce * dashCurve.Evaluate(t));
            vel.y = 0f;

            rb.linearVelocity = vel;
            return;
        }

        rb.linearVelocity = vel;
    }

    public void StopUpdate()
    {
        rb.linearVelocity = Vector3.zero;
    }

    void JumpUpdate()
    {
        if (jumpTimer > 0f)
        {
            jumpTimer = Mathf.Max(0f, jumpTimer - Time.deltaTime);
            jumping = true;
        }
        else jumping = false;

        // Coyote
        if (touchingGround) coyoteTimer = 0f;
        else if (wasOnGround && !jumping) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        if (touchingGround && !wasOnGround) currentNumberOfAirJump = numberOfAirJump;
        wasOnGround = touchingGround;

        // Buffer
        if (Input.GetKeyDown(KeyCode.Space)) jumpBufferTimer = jumpBufferTime;
        else jumpBufferTimer = Mathf.Max(0f, jumpBufferTimer - Time.deltaTime);

        bool canJump = touchingGround || coyoteTimer > 0f || currentNumberOfAirJump > 0;
        if (jumpBufferTimer > 0f && canJump)
        {
            if (!touchingGround && coyoteTimer > 0f) coyoteTimer = 0f;
            else if (!touchingGround) currentNumberOfAirJump--;

            jumpBufferTimer = 0f;
            jumpTimer = 0f;
            Jump();
        }
    }


    void Jump()
    {
        jumpTimer = jumpDuration;

        //jump direction
        Vector3 baseMove = moveVector * currentSpeed;
        jumpDirection = new Vector3(baseMove.x, 0f, baseMove.z);

    }

    void GroundUpdate()
    {
        touchingGround = false;

        Vector3 start = transform.position + (transform.up * botRayHeight);
        Vector3 down = -transform.up;

        Vector3[] offsets = new Vector3[]
        {
        new Vector3( botRayOffset, 0,  botRayOffset),
        new Vector3(-botRayOffset, 0,  botRayOffset),
        new Vector3( botRayOffset, 0, -botRayOffset),
        new Vector3(-botRayOffset, 0, -botRayOffset),
        };

        foreach (var offset in offsets)
        {
            Ray ray = new Ray(start + offset, down);
            Debug.DrawRay(ray.origin, ray.direction * botRaySize);
            if (Physics.Raycast(ray, botRaySize, ~ignoredLayer))
                touchingGround = true;
        }
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;
        float elapsed = 0f;

        while (elapsed < dashCooldown || !touchingGround)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        canDash = true;
    }

    public void Die()
    {

    }

    public void UnlockDoubleJump()
    {
        numberOfAirJump = 1;
    }
}
