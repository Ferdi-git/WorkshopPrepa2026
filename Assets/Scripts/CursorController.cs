using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Image cursorImage;
    public float fadeOpacity;
    public void DisplayUpdate()
    {
        if(HUD.Instance.Displaying())
        {
            HUDMode();
        }
        else
        {
            NormalMode();
        }


    }

    void HUDMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cursorImage.gameObject.SetActive(false);


    }

    void NormalMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cursorImage.gameObject.SetActive(true);
        cursorImage.color = Color.white;




        if (PlayerObjectAnimation.Instance.hideCursor)
        {
            cursorImage.gameObject.SetActive(false);
            return;
        }

        if (PlayerObjectAnimation.Instance.fadeCursor)
        {
            cursorImage.color = new Color(1, 1, 1, fadeOpacity);
        }
    }

    public void InteractAnim()
    {
        cursorImage.transform.localScale = Vector3.one * 1.3f;
        cursorImage.transform.DOScale(Vector3.one, 0.2f);
    }
}
