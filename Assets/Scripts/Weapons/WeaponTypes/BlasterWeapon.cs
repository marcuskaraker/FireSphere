using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Blaster", menuName ="Weapon/Blaster")]
public class BlasterWeapon : Weapon
{
    [Header("Blaster Settings")]
    public Projectile projectilePrefab;

    public bool useRelativeBulletSpeed;
    public bool targetClosestEnemy;

    public override bool Fire(Shooter shooter)
    {
        Projectile[] spawnedProjectiles = null;
        switch (barrelFireMode)
        {
            case BarrelFireMode.FireAll:
                spawnedProjectiles = GameManager.Instance.ProjectileManager.SpawnBullets(
                    projectilePrefab, 
                    shooter.firePositions, 
                    shooter.LatestRelativeVelocity,
                    useRelativeBulletSpeed, 
                    projectileData
                );
                break;
            case BarrelFireMode.TurnFire:
                spawnedProjectiles = new Projectile[1];
                spawnedProjectiles[0] = GameManager.Instance.ProjectileManager.SpawnBullet(
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

        // Targeting
        if (targetClosestEnemy)
        {
            for (int i = 0; i < spawnedProjectiles.Length; i++)
            {
                EnemyController closestEnemy = GameManager.Instance.GetClosestEnemyToPos(shooter.transform.position);
                if (closestEnemy != null)
                {
                    spawnedProjectiles[i].target = closestEnemy.transform;
                }               
            }
        }

        shooter.currentClipSize -= 1f;

        return true;
    }
}
