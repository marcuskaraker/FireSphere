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

    public override void Interact(GameObject target, InstancePickupData instancePickupData)
    {
        Shooter shooter = target.GetComponent<Shooter>();
        if (shooter)
        {
            if (instancePickupData is InstanceWeaponPickupData instanceWeaponPickupData)
            {
                shooter.EquipWeapon(weapon, instanceWeaponPickupData.durability);
            }
            else
            {
                shooter.EquipWeapon(weapon);
            }
        }
    }

    public override InstancePickupData NewInstanceData()
    {
        return new InstanceWeaponPickupData(weapon.durability);
    }
}
