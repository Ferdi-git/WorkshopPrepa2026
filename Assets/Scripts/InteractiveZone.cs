using UnityEngine;

public class InteractiveZone : InteractiveObject
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerInteractor.Instance.Interact(this);
        }
    }
}
