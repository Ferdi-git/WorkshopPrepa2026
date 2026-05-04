using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string currentCheckpointName;

    Vector3 basePlayerPos;

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        basePlayerPos = PlayerController.Instance.transform.position;
        Initialize();
    }

    public virtual void Initialize()
    {
        PlayerInteractor.Instance.Initialize();
        HUD.Instance.Initialize();
    }

    private void Update()
    {
        //player
        if(!HUD.Instance.Displaying())
        {
            PlayerController.Instance.PlayerControlUpdate();

            CameraController.Instance.LookUpdate();
            CameraController.Instance.AnimationUpdate();

            PlayerInteractor.Instance.SelectionUpdate();
            PlayerInteractor.Instance.InteractionUpdate();
        }

        PlayerObjectManager.Instance.HoldingUpdate();
        PlayerObjectAnimation.Instance.AnimationUpdate();

        CursorController.Instance.DisplayUpdate();
        HUD.Instance.DisplayUpdate();
    }



    private void FixedUpdate()
    {
        if (!HUD.Instance.Displaying())
        {
            PlayerController.Instance.MoveUpdate();
        }
        else
        {
            PlayerController.Instance.StopUpdate();
        }
    }

    public void SetCheckpoint(string objectName)
    {
        currentCheckpointName = objectName;
    }

    public void KillPlayer()
    {
        if(PlayerInteractor.Instance.InteractiveObjectFromName(currentCheckpointName) != null)
        {
            PlayerController.Instance.transform.position = PlayerInteractor.Instance.InteractiveObjectFromName(currentCheckpointName).transform.position;
            return;
        }

        PlayerController.Instance.transform.position = basePlayerPos;

    }
}
