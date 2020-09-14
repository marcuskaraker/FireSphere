using UnityEngine;

public enum BarrelFireMode { FireAll, TurnFire }
public enum FireMode { Single, Automatic, Burst }

//[CreateAssetMenu(fileName = "Weapon_(WEAPON_NAME)", menuName = "Weapon/(WEAPON_NAME)")]
public abstract class Weapon : ScriptableObject
{
    [Header("Weapon Settings")]
    public Sprite weaponIcon;
    public Color weaponIconColor = Color.white;
    public Pickup weaponDropPickup;
    public BarrelFireMode barrelFireMode;
    public FireMode fireMode;

    public ProjectileData projectileData;

    [Space]
    public float durability = -1;
    public float range = 10f;
    public float fireRate = 0.1f;
    public float clipSize = 50f;
    public float recoil = 0.5f;

    public abstract bool Fire(Shooter shooter);

    public virtual void Reload(Shooter shooter)
    {
        shooter.currentClipSize = clipSize;
    }
}
