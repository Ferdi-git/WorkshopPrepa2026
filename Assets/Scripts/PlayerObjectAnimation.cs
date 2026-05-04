using DG.Tweening;
using UnityEngine;

public class PlayerObjectAnimation : MonoBehaviour
{
    public static PlayerObjectAnimation Instance;

    public Transform animationParent;

    [Header("Running")]
    public bool canSwitch = true;
    public float runningInSpeed, runningOutSpeed;

    [Header("Jumping")]
    public float jumpRecoveryLength;
    bool waitingForRecovery;
    float jumpRecoveryTimer;

    [Header("Held Objects")]
    [SerializeField] Vector3 objectPos;
    [SerializeField] Vector3 objectEul;
    [SerializeField] Vector3 runningObjectPos;
    [SerializeField] Vector3 runningObjectEul;


    [Header("Leaning")]
    public Transform leaningParent;
    public float leftLean;
    public float rightLean;
    public float leanSpeed;

    Vector3 targetLean = Vector3.zero;

    [Header("Swaying")]
    public Transform swayingTransform;
    public float swayStrength;
    public float aimingSwayStrength;
    Vector3 swayPosRef;
    public float swaySmoothSpeed;

    [Header("Bobbing")]
    public Transform bobbingParent;
    Vector3 bob;

    public float bobbingSmoothSpeed;
    public float walkBobFrequency;
    public float walkBobAmplitude;

    public float runBobFrequency;
    public float runBobAmplitude;

    public AnimationCurve jumpBobYCurve;


    Vector3 transitionRef;

    private void Awake()
    {
        Instance = this;
    }


    public void AnimationUpdate()
    {
        PoseUpdate();
        LeanUpdate();
        SwayUpdate();
        BobbingUpdate();
    }

    public bool runningAnimation, aimingAnimation;
    public bool hideCursor;
    public bool fadeCursor;
    public void PoseUpdate()
    {
        if (!PlayerController.Instance.touchingGround)
        {
            waitingForRecovery = true;

            if (PlayerController.Instance.applyingRunForce)
            {
                if (!runningAnimation)
                {
                    RunningPose();
                    return;
                }
            }
            else
            {
                if (runningAnimation)
                {
                    StopRunningPos();
                }
                return;
            }


        }
        else
        {
            if (waitingForRecovery)
            {
                jumpRecoveryTimer = jumpRecoveryLength;
                waitingForRecovery = false;
            }
            else
            {
                if (jumpRecoveryTimer > 0f)
                {
                    jumpRecoveryTimer -= Time.deltaTime;
                    if (jumpRecoveryTimer <= 0f) jumpRecoveryTimer = 0f;
                }
            }
        }

        if (PlayerController.Instance.applyingRunForce)
        {
            if (!runningAnimation)
            {
                if (jumpRecoveryTimer <= 0f)
                {
                    RunningPose();
                }
            }

            return;
        }
        else
        {
            if (runningAnimation)
            {
                StopRunningPos();
            }
        }
    }

    Tween posTween, eulTween;

    public void RunningPose()
    {
        //Debug.Log("running pose");

        posTween.Kill();
        eulTween.Kill();

        canSwitch = true;

        ResetBob();

        runningAnimation = true;
        aimingAnimation = false;

        fadeCursor = true;

        float speed = runningInSpeed;

        posTween = animationParent.transform.DOLocalMove(runningObjectPos, speed);
        eulTween = animationParent.transform.DOLocalRotate(runningObjectEul, speed);
    }

    public void StopRunningPos()
    {
        hideCursor = false;
        fadeCursor = false;

        posTween.Kill();
        eulTween.Kill();

        canSwitch = true;

        ResetBob();

        float speed = runningOutSpeed;

        posTween = animationParent.transform.DOLocalMove(objectPos, speed);
        eulTween = animationParent.transform.DOLocalRotate(objectEul, speed);
        runningAnimation = false;
    }






    public void LeanUpdate()
    {
        Vector3 newTarget = Vector3.zero;

        if (PlayerController.Instance.input.x < 0)
        {
            newTarget.z = leftLean;
        }
        else if (PlayerController.Instance.input.x > 0)
        {
            newTarget.z = rightLean;
        }

        if (targetLean != newTarget)
        {
            targetLean = newTarget;
            leaningParent.DOLocalRotate(targetLean, leanSpeed);
        }

    }



    public void BobbingUpdate()
    {
        Vector3 localPos = Vector3.zero;

        if (PlayerController.Instance.jumping)
        {
            localPos = JumpBob();
        }
        else if (PlayerController.Instance.moving)
        {
            if (PlayerController.Instance.applyingRunForce)
            {
                localPos = RunBob();
            }
            else
            {
                localPos = WalkBob();
            }

        }

        if (localPos == Vector3.zero)
        {
            ResetBob();
        }

        bobbingParent.transform.localPosition = Vector3.SmoothDamp(bobbingParent.transform.localPosition, localPos, ref transitionRef, bobbingSmoothSpeed);
    }

    //public float stabilizePointDistance;
    void ResetBob()
    {
        bob = Vector3.zero;
    }

    Vector3 WalkBob()
    {
        bob.y += Mathf.Sin(Time.time * walkBobFrequency) * walkBobAmplitude;
        bob.x += Mathf.Cos(Time.time * walkBobFrequency / 2) * walkBobAmplitude / 2;
        return bob;
    }

    Vector3 RunBob()
    {
        bob.y += Mathf.Sin(Time.time * runBobFrequency) * runBobAmplitude;
        bob.x += Mathf.Cos(Time.time * runBobFrequency / 2) * runBobAmplitude / 2;
        return bob;
    }

    Vector3 JumpBob()
    {
        return new Vector3(0, jumpBobYCurve.Evaluate(1f - (PlayerController.Instance.jumpTimer / PlayerController.Instance.jumpDuration)), 0);
    }


    public void SwayUpdate()
    {
        float strength = swayStrength;

        float x = CameraController.Instance.mouseInputVector.x* strength;
        swayingTransform.localPosition = Vector3.SmoothDamp(swayingTransform.localPosition, new Vector3(x, 0f, 0f), ref swayPosRef, swaySmoothSpeed); 
    }
}
