using UnityEngine;

public abstract class Pickup : ScriptableObject
{
    public string pickupName;

    public abstract void Interact(GameObject target);
}
