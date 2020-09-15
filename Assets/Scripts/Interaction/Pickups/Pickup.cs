using UnityEngine;

public abstract class Pickup : ScriptableObject
{
    public string pickupName;
    public Sprite icon;
    public Color iconColor = Color.white;

    public abstract void Interact(GameObject target, InstancePickupData instancePickupData);
    public abstract InstancePickupData NewInstanceData();
}
