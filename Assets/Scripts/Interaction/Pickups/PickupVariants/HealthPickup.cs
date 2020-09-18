using UnityEngine;
using MK.Destructible;

[CreateAssetMenu(fileName = "Pickup_Health_", menuName = "Pickups/Health")]
public class HealthPickup : Pickup
{
    public float health = 10f;

    public override bool Interact(GameObject target, InstancePickupData instancePickupData)
    {
        Destructible destructible = target.GetComponent<Destructible>();
        if (destructible)
        {
            destructible.Hurt(-health);
            return true;
        }

        return false;
    }

    public override InstancePickupData NewInstanceData()
    {
        return null;
    }
}
