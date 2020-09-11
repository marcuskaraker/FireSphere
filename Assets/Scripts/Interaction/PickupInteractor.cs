using UnityEngine;

public class PickupInteractor : MonoBehaviour
{
    private const string PICKUP_TAG = "Pickup";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PICKUP_TAG))
        {
            collision.GetComponent<PickupInteractable>().Interact(gameObject);
        }
    }
}
