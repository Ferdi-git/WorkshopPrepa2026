using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        elements = FindObjectsByType<HUDElement>(FindObjectsSortMode.None);

        HideAllElements();
    }

    [SerializeField] TextMeshProUGUI tm_context;

    [SerializeField] GameObject backButton;

    HUDElement[] elements;

    public void DisplayUpdate()
    {
        tm_context.text = "";

        if(PlayerInteractor.Instance.objectSelected != null)
        {
            tm_context.text = PlayerInteractor.Instance.objectSelected.objectName;
        }
    }

    public void DisplayElement(string elementName)
    {
        HideAllElements();

        for (int i = 0; i < elements.Length; i++)
        {
            if(elements[i].elementName == elementName)
            {
                elements[i].gameObject.SetActive(true);
                backButton.SetActive(true);
            }
        }


    }

    public void HideAllElements()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            elements[i].gameObject.SetActive(false);
        }

        backButton.SetActive(false);
    }

    public bool Displaying()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if(elements[i].gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }
}
