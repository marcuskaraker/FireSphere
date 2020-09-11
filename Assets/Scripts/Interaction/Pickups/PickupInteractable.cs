using UnityEngine;

public class PickupInteractable : MonoBehaviour
{
    public Pickup pickupData;

    public void Interact(GameObject target)
    {
        pickupData.Interact(target);
        Destroy(gameObject);
    }
}
