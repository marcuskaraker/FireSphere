﻿using System.Collections;
using UnityEngine;

public class PickupInteractable : MonoBehaviour
{
    public Pickup pickupData;
    public float enableWaitTime = 1f;
    public bool isInteractable;
    public InstancePickupData instancePickupData;

    [Space]
    public SpriteRenderer iconRenderer;

    private void Awake()
    {
        instancePickupData = pickupData.NewInstanceData();

        StartCoroutine(DoEnableAfterTime());
    }

    public bool Interact(GameObject target)
    {
        if (!isInteractable)    
        {
            return false;
        }

        bool pickupSuccess = pickupData.Interact(target, instancePickupData);

        if (pickupSuccess)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
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
