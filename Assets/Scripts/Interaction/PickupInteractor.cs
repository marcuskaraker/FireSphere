using UnityEngine;

public class PickupInteractor : MonoBehaviour
{
    private const string PICKUP_TAG = "Pickup";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PICKUP_TAG))
        {
            PickupInteractable pickup = collision.GetComponent<PickupInteractable>();
            pickup.Interact(gameObject);

            GameManager.Instance.UIManager.PromptIfEmpty(pickup.pickupData.pickupName);
        }
    }
}
