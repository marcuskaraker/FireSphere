using MK.Audio;
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
                GameManager.Instance.UIManager.PromptIfEmpty(1, MK.UI.TransitionPreset.ScaleIn, pickup.iconRenderer.sprite, pickup.pickupData.pickupName);
                AudioManager.PlayOneShot(
                    GameManager.Instance.AudioData.pickupAudio, 
                    transform.position, 
                    GameManager.Instance.AudioData.pickupAudioVolume, 
                    GameManager.Instance.AudioData.audioSpatialBlend
                );
            }           
        }
    }
}
