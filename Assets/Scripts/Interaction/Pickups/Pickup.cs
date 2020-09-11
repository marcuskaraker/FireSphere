using UnityEngine;

public abstract class Pickup : ScriptableObject
{
    public abstract void Interact(GameObject target);
}
