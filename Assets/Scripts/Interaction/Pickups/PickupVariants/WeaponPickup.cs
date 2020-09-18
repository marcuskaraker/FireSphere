using UnityEngine;

public class InstanceWeaponPickupData : InstancePickupData
{
    public float durability;

    public InstanceWeaponPickupData(float durability)
    {
        this.durability = durability;
    }
}

[CreateAssetMenu(fileName = "Pickup_Weapon_", menuName = "Pickups/Weapon")]
public class WeaponPickup : Pickup
{
    public Weapon weapon;

    public override bool Interact(GameObject target, InstancePickupData instancePickupData)
    {
        Shooter shooter = target.GetComponent<Shooter>();
        if (shooter)
        {
            if (instancePickupData is InstanceWeaponPickupData instanceWeaponPickupData)
            {
                return shooter.EquipWeaponAtFirstAvailableSlot(weapon, instanceWeaponPickupData.durability);
            }
            else
            {
                return shooter.EquipWeaponAtFirstAvailableSlot(weapon);
            }          
        }

        return false;
    }

    public override InstancePickupData NewInstanceData()
    {
        return new InstanceWeaponPickupData(weapon.durability);
    }
}
