using System.Collections;
using UnityEngine;

public class PickupInteractable : MonoBehaviour
{
    public Pickup pickupData;
    public float enableWaitTime = 1f;
    public bool isInteractable;

    [Space]
    public SpriteRenderer iconRenderer;

    private void Awake()
    {
        StartCoroutine(DoEnableAfterTime());
    }

    public void Interact(GameObject target)
    {
        if (!isInteractable)    
        {
            return;
        }

        pickupData.Interact(target);
        Destroy(gameObject);
    }

    private IEnumerator DoEnableAfterTime()
    {
        yield return new WaitForSeconds(enableWaitTime);

        isInteractable = true;
    }

    public void SetIcon(Sprite sprite, Color color)
    {
        iconRenderer.sprite = sprite;
        iconRenderer.color = color;
    }
}
