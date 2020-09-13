using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Blaster", menuName ="Weapon/Blaster")]
public class BlasterWeapon : Weapon
{
    [Header("Blaster Settings")]
    public Projectile projectilePrefab;

    public bool useRelativeBulletSpeed;

    public override bool Fire(Shooter shooter)
    {
        switch (barrelFireMode)
        {
            case BarrelFireMode.FireAll:
                GameManager.Instance.SpawnBullets(
                    projectilePrefab, 
                    shooter.firePositions, 
                    shooter.LatestRelativeVelocity,
                    useRelativeBulletSpeed, 
                    projectileData
                );
                break;
            case BarrelFireMode.TurnFire:
                GameManager.Instance.SpawnBullet(
                    projectilePrefab, 
                    shooter.firePositions, 
                    shooter.firePointIndex, 
                    shooter.LatestRelativeVelocity, 
                    useRelativeBulletSpeed, 
                    projectileData
                );
                shooter.firePointIndex = (shooter.firePointIndex + 1) % shooter.firePositions.Length;
                break;
        }

        shooter.currentClipSize -= 1f;

        return true;
    }
}
