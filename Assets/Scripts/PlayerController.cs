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

    [SerializeField] float speed;
    [SerializeField] float acc;
    [SerializeField] float runForce;

    Vector2 smoothedInput;
    Vector2 inputVelocity;

    [Header("Jumping")]
    [SerializeField] float botRayHeight;
    [SerializeField] float botRaySize;
    [SerializeField] float botRayOffset;
    public float jumpDuration;

    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] AnimationCurve jumpDirectionCurve;

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
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    readyToRun = !readyToRun;

                    if (running)
                    {
                        StopRunning();

                        readyToRun = false;
                    }


                }

            }
            else
            {
                if(running)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        StopRunning();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift) || readyToRun)
                    {
                        StartRunning();
                    }
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

        if (input.magnitude > 0) moving = true;
        else
        {
            if (running) StopRunning();

            if(Input.GetKey(KeyCode.LeftShift))
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

        vel = moveVector * speed;

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

        rb.linearVelocity = vel;
    }

    public void StopUpdate()
    {
        rb.linearVelocity = Vector3.zero;
    }

    void JumpUpdate()
    {
        jumping = false;

        if (jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f) jumpTimer = 0f;

            jumping = true;
        }
        else
        {
            jumping = false;
        }
 
        //touchingGround = Physics.BoxCast(transform.position + (transform.up * botRayHeight), Vector3.one * botRaySize / 2f, -transform.up, Quaternion.identity, botRaySize, ~ignoredLayer); ;

        if(touchingGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        jumpTimer = jumpDuration;

        //jump direction
        jumpDirection = vel;

    }

    void GroundUpdate()
    {
        touchingGround = false;

        Ray[] rays = new Ray[4];
        Vector3 start = transform.position + (transform.up * botRayHeight);
        Vector3 offset = new Vector3(botRayOffset, 0, botRayOffset);
        rays[0] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(-botRayOffset, 0, botRayOffset);
        rays[1] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(botRayOffset, 0, -botRayOffset);
        rays[2] = new Ray(start + offset, -transform.up + offset);
        offset = new Vector3(-botRayOffset, 0, -botRayOffset);
        rays[3] = new Ray(start + offset, -transform.up + offset);

        for (int i = 0; i < 4; i++)
        {
            Debug.DrawRay(rays[i].origin, rays[i].direction*botRaySize);
            if(Physics.Raycast(rays[i], botRaySize, ~ignoredLayer))
            {
                touchingGround = true;
            }
        }

        //touchingGround = Physics.Raycast(transform.position + (transform.up * botRayHeight), -transform.up, botRaySize, ~ignoredLayer);

    }

    public void Die()
    {

    }
}
