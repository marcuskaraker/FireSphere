using UnityEngine;
using MK.Destructible;

[CreateAssetMenu(fileName = "Pickup_Health_", menuName = "Pickups/Health")]
public class HealthPickup : Pickup
{
    public float health = 10f;

    public override void Interact(GameObject target)
    {
        Destructible destructible = target.GetComponent<Destructible>();
        if (destructible)
        {
            destructible.Hurt(-health);
        }
    }
}
