using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerObjectManager : MonoBehaviour
{
    public static PlayerObjectManager Instance;

    private void Awake()
    {
        Instance = this;

        objectsParent.transform.parent = parentParent;
        objectsParent.transform.localPosition = Vector3.zero;
        objectsParent.transform.localEulerAngles = Vector3.zero;
        objectsParent.transform.localScale = Vector3.one;
    }

    [SerializeField] GameObject objectsParent;
    [SerializeField] Transform parentParent;

    public PlayerObject[] playerObjects;
    int heldObjectID = -1;

    bool switching;

    [SerializeField] float switchingSpeed;

    public PlayerObject HeldObject()
    {
        if (heldObjectID < 0) return null;

        return playerObjects[heldObjectID];
    }

    int PossessedObjectCount()
    {
        int count = 0;
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if(playerObjects[i].possessed)
            {
                count++;
            }
        }

        return count;
    }

    public void TryGetObject(string objectName)
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if(objectName == playerObjects[i].objectName)
            {
                GetObject(playerObjects[i]);
                return;
            }
        }

        Debug.LogError("No object by name " + objectName);

        return;
    }

    public void TryLooseObject(string objectName)
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            if (objectName == playerObjects[i].objectName)
            {
                LooseObject(playerObjects[i]);
                return;
            }
        }

        Debug.LogError("No object by name " + objectName);

        return;
    }

    public void HoldingUpdate()
    {
        for (int i = 0; i < playerObjects.Length; i++)
        {
            playerObjects[i].gameObject.SetActive(playerObjects[i].possessed);
        }

        if(switching)
        {
            return;
        }

        if (PossessedObjectCount() < 1) return;

        if(PossessedObjectCount() > 1)
        {
            if (Input.mouseScrollDelta.y < 0f)
            {
                StartCoroutine(SwitchCoroutine(true));
            }
            else if (Input.mouseScrollDelta.y > 0f)
            {
                StartCoroutine(SwitchCoroutine(false));
            }
        }


    }  
    
    void TryUnholdObject()
    {
        if (heldObjectID >= 0)
        {
            playerObjects[heldObjectID].transform.DOLocalMoveZ(-2f, switchingSpeed);
        }
    }

    IEnumerator SwitchCoroutine(bool previous)
    {
        TryUnholdObject();

        if (previous)
        {
            Debug.Log("Switching to previous");

            if(heldObjectID < 0)
            {
                for (int i = playerObjects.Length-1; i >= 0; i--)
                {
                    if(playerObjects[i].possessed)
                    {
                        heldObjectID = i;
                        break;
                    }
                }
            }
            else
            {
                bool found = false;

                for (int i = heldObjectID-1; i >= 0; i--)
                {
                    if (playerObjects[i].possessed)
                    {
                        found = true;
                        heldObjectID = i;
                        break;
                    }
                }

                if(!found)
                heldObjectID = -1;
            }
        }
        else
        {
            Debug.Log("Switching to next");

            bool found = false;

            for (int i = heldObjectID + 1; i < playerObjects.Length; i++)
            {
                Debug.Log("Checking " + playerObjects[i]);

                if (playerObjects[i].possessed)
                {
                    Debug.Log("Found " + i);
                    found = true;
                    heldObjectID = i;
                    break;
                }
            }

            if (!found)
                heldObjectID = -1;

            Debug.Log("Held id = " + heldObjectID);
        }


        if(heldObjectID >= 0)
        {
            playerObjects[heldObjectID].transform.DOLocalMoveZ(0f, switchingSpeed);
        }

        yield return new WaitForSeconds(switchingSpeed);

        switching = false;
    }

    void GetObject(PlayerObject objectGotten)
    {
        objectGotten.possessed = true;

        switching = true;

        TryUnholdObject();

        for (int i = 0; i < playerObjects.Length; i++)
        {
            if(playerObjects[i] == objectGotten)
            {
                heldObjectID = i;
                playerObjects[i].transform.localPosition = new Vector3(0f, 0f, -2f);
                playerObjects[i].transform.DOLocalMoveZ(0f, switchingSpeed).OnComplete(() =>
                {
                    switching = false;
                });
            }
        }
    }

    void LooseObject(PlayerObject objectLost)
    {
        objectLost.possessed = false;
        heldObjectID = -1;
    }
}
