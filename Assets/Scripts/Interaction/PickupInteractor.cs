using UnityEngine;

public class PickupInteractor : MonoBehaviour
{
    private const string PICKUP_TAG = "Pickup";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PICKUP_TAG))
        {
            PickupInteractable pickup = collision.GetComponent<PickupInteractable>();
            bool didPickup = pickup.Interact(gameObject);

            if (didPickup)
            {
                GameManager.Instance.UIManager.PromptIfEmpty(1, pickup.iconRenderer.sprite, pickup.pickupData.pickupName);
            }           
        }
    }
}
