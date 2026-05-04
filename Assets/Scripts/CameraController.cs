using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;


    private void Awake()
    {
        Instance = this;
    }

    public float mouseSensitivity = 1f;
    public float clampAngle = 80.0f;

    public Transform lookingTransform;
    public Transform cameraTransform;

    [HideInInspector]
    public float rotY = 0.0f; // rotation around the up/y axis

    [HideInInspector]
    public float rotX = 0.0f; // rotation around the right/x axis

    public float normalZoom;
    public float runningZoom;
    public float runningZoomSpeed;

    void Start()
    {
        currentZoom = normalZoom;
        zoomSpeed = runningZoomSpeed;
    }


    public Vector2 mouseInputVector;

    public void LookUpdate()
    {


        mouseInputVector.x = Input.GetAxis("Mouse X");
        mouseInputVector.y = -Input.GetAxis("Mouse Y");

        rotY += mouseInputVector.x * mouseSensitivity;
        rotX += mouseInputVector.y * mouseSensitivity;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        lookingTransform.localRotation = localRotation;
    }

    public void AnimationUpdate()
    {
        Vector3 localPos = ResetAnimation();

        if(PlayerController.Instance.jumping)
        {
            localPos = JumpAnimation();
            runningBob = Vector3.zero;
        }
        else if (PlayerController.Instance.moving && PlayerController.Instance.touchingGround)
        {
            if (PlayerController.Instance.applyingRunForce)
            {
                if(PlayerController.Instance.rb.linearVelocity.magnitude >= runningBobVelThreshold)
                localPos = RunAnimation();
                else runningBob = localPos;

            }
            else
            {
                runningBob = localPos;
            }
        }

        cameraAnimationTransform.transform.localPosition = localPos;

        ZoomUpdate();
        //stabilize
        //cameraTransform.LookAt(transform.position + (lookingTransform.forward * stabilizePointDistance));
    }

    float currentZoom;
    float zoomSpeed;
    
    public void ZoomUpdate()
    {
        float zoom = normalZoom;

        zoomSpeed = runningZoomSpeed;

        if (PlayerController.Instance.applyingRunForce)
        {
            zoom = runningZoom;
        }

        currentZoom = Mathf.MoveTowards(currentZoom, zoom, zoomSpeed * Time.deltaTime);
        Camera.main.fieldOfView = currentZoom;
    }

    [Header("Animation")]
    public GameObject cameraAnimationTransform;
    public float resettingSpeed;
    Vector3 resetRef;

    public float runningBobVelThreshold;
    public float runningBobFrequency;
    public float runningBobAmplitude;

    public AnimationCurve jumpAnimYCurve;

    //public float stabilizePointDistance;
    Vector3 ResetAnimation()
    {
        return Vector3.SmoothDamp(cameraAnimationTransform.transform.localPosition, Vector3.zero, ref resetRef, resettingSpeed);
    }

    Vector3 WalkAnimation()
    {
        return Vector3.zero;
    }

    Vector3 runningBob = Vector3.zero;
    Vector3 RunAnimation()
    {
        runningBob.y += Mathf.Sin(Time.time * runningBobFrequency) * runningBobAmplitude;
        runningBob.x += Mathf.Cos(Time.time * runningBobFrequency / 2) * runningBobAmplitude / 2;
        return runningBob;
    }

    Vector3 JumpAnimation()
    {
        return new Vector3(0, jumpAnimYCurve.Evaluate(1f - (PlayerController.Instance.jumpTimer/PlayerController.Instance.jumpDuration)), 0);
    }

}
