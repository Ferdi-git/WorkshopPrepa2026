using UnityEngine;
using System.Collections;

public class PlayerInteractor : MonoBehaviour
{
    public static PlayerInteractor Instance;

    public AudioSource audioSource;

    public AudioClip[] deathSounds;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize()
    {
        interactiveObjects = FindObjectsByType<InteractiveObject>(FindObjectsSortMode.None);
    }

    [HideInInspector]
    public InteractiveObject objectSelected;
    [HideInInspector]
    public InteractiveObject objectInteracting;
    Interaction interaction;


    [SerializeField] Transform cameraLookerTransform;
    [SerializeField] float selectionDistance;

    InteractiveObject[] interactiveObjects;

    public void SelectionUpdate()
    {
        if(objectInteracting != null)
        {
            objectSelected = null;

            return;
        }

        RaycastHit hit;
        Ray ray = new Ray(cameraLookerTransform.position, cameraLookerTransform.forward);
        Debug.DrawRay(cameraLookerTransform.position, cameraLookerTransform.forward);
        if (Physics.Raycast(ray, out hit, selectionDistance))
        {
            objectSelected = hit.collider.GetComponent<InteractiveObject>();

            if(objectSelected != null && objectSelected.interactions.Length == 0) 
                objectSelected = null;
        }
        else
        {
            objectSelected = null;
        }

        
    }

    public void InteractionUpdate()
    {
        if (objectSelected == null)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            Interact(objectSelected);
        }
    }

    public void Interact(InteractiveObject interactiveObject)
    {
        Debug.Log("Interacting with " + interactiveObject.objectName);

        objectInteracting = interactiveObject;
        interaction = objectInteracting.interactions[objectInteracting.interactionID];
        
        if(interaction.requiredObject != "")
        {
            if(!PlayerObjectManager.Instance.HeldObject())
            {
                Debug.Log("Cannot interact - " + interaction.requiredObject + " missing!");
                ResetInteraction();
                return;
            }

            if(PlayerObjectManager.Instance.HeldObject().objectName != interaction.requiredObject)
            {
                Debug.Log("Cannot interact - " + interaction.requiredObject + " missing!");
                ResetInteraction();
                return;
            }

        }


        if (objectInteracting.interactionID == interaction.nextInteractionID)
        {
            interaction.playNextInteractionInstantly = false;
        }

        objectInteracting.interactionID = interaction.nextInteractionID;


        CursorController.Instance.InteractAnim();

        switch(interaction.type)
        {
            case InteractionType.getPlayerObject:
                PlayerObjectManager.Instance.TryGetObject(interaction.stringArg);
                break;
            case InteractionType.loosePlayerObject:
                PlayerObjectManager.Instance.TryLooseObject(interaction.stringArg);
                break;
            case InteractionType.goAway:
                objectInteracting.GoAway();
                break;
            case InteractionType.sendInteractiveObjectAway:
                for (int i = 0; i < interactiveObjects.Length; i++)
                {
                    if(interactiveObjects[i].objectName == interaction.stringArg)
                    {
                        interactiveObjects[i].GoAway();
                        break;
                    }
                }
                break;
            case InteractionType.placeInteractiveObject:
                for (int i = 0; i < interactiveObjects.Length; i++)
                {
                    if (interactiveObjects[i].objectName == interaction.stringArg)
                    {
                        interactiveObjects[i].Place();
                        break;
                    }
                }
                break;
            case InteractionType.displayHUDElement:
                HUD.Instance.DisplayElement(interaction.stringArg);
                break;
            case InteractionType.playSound:
                SoundManager.Instance.PlaySound(interaction.stringArg);
                break;
            case InteractionType.setCheckpoint:
                GameManager.Instance.SetCheckpoint(interaction.stringArg);
                break;
            case InteractionType.killPlayer:

                int randInt = Random.Range(0, deathSounds.Length);
                audioSource.clip = deathSounds[randInt];
                audioSource.Play();

                GameManager.Instance.KillPlayer();

                break;
            case InteractionType.teleportPlayer:
                PlayerController.Instance.transform.position = InteractiveObjectFromName(interaction.stringArg).transform.position;
                PlayerController.Instance.transform.rotation = InteractiveObjectFromName(interaction.stringArg).transform.rotation;

                break;
            case InteractionType.unlockCapacity:

                if (interaction.stringArg == "Double Jump")
                {
                    PlayerController.Instance.numberOfAirJump = 1;
                }
                else if(interaction.stringArg == "Sprint")
                {
                    PlayerController.Instance.unlockedSprint = true;

                }
                else if (interaction.stringArg == "Crouch")
                {
                    PlayerController.Instance.unlockedCrouch = true;

                }
                else if (interaction.stringArg == "Dash")
                {
                    PlayerController.Instance.unlockedDash = true;

                }
                else if (interaction.stringArg == "Shield")
                {

                }
                else if (interaction.stringArg == "67")
                {

                }
                else
                {
                    Debug.LogError("Salut c'est Ferdi t'as mis une valeur inconue dans ton unlockCapa les options c'est Double Jump, Sprint, Crouch, Sonic, Michael, Shield, 67");
                }
                break;
            case InteractionType.playAnimation:
                Animator anim = interactiveObject.GetComponent<Animator>();
                if(anim == null)
                {
                    anim = interactiveObject.GetComponentInParent<Animator>();

                }
                
                anim.SetTrigger(interaction.stringArg);
                break;
        }

        StartCoroutine(InteractionCoroutine());
    }



    IEnumerator InteractionCoroutine()
    {
        yield return new WaitForSeconds(interaction.length);

        while(HUD.Instance.Displaying())
        {
            yield return new WaitForEndOfFrame();
        }

        EndInteraction();
    }

    void EndInteraction()
    {
        if(interaction.playNextInteractionInstantly)
        {
            Interact(objectInteracting);
        }
        else
        {
            ResetInteraction();
        }
    }

    void ResetInteraction()
    {
        objectInteracting = null;
        interaction = null;
    }

    public InteractiveObject InteractiveObjectFromName(string objectName)
    {
        for (int i = 0; i < interactiveObjects.Length; i++)
        {
            if(interactiveObjects[i].objectName == objectName)
            {
                return interactiveObjects[i];
            }
        }

        
        return null;
    }
}
