using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public string objectName;

    public int interactionID;
    public Interaction[] interactions;

    public bool goAwayAtStart;

    Vector3 basePosition;

    private void Awake()
    {
        basePosition = transform.position;

        if (goAwayAtStart)
        {
            GoAway();
        }
    }


    public void GoAway()
    {
        transform.position = new Vector3(999f, 999f, 999f);
    }

    public void Place()
    {
        transform.position = basePosition;
    }
}
