using UnityEngine;

[CreateAssetMenu(fileName = "Pickup_Weapon_", menuName = "Pickups/Weapon")]
public class WeaponPickup : Pickup
{
    public Weapon weapon;

    public override void Interact(GameObject target)
    {
        Shooter shooter = target.GetComponent<Shooter>();
        if (shooter)
        {
            shooter.EquipWeapon(weapon);
        }
    }
}
