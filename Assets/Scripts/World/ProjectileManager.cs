using MK;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<Projectile> currentProjectiles;

    [SerializeField] int projectilePoolSize = 300;
    [SerializeField] Projectile[] allProjectilePrefabs;

    private void Awake()
    {
        currentProjectiles = new List<Projectile>();
    }

    private void Start()
    {
        for (int i = 0; i < allProjectilePrefabs.Length; i++)
        {
            ObjectPoolManager.CreatePool(allProjectilePrefabs[i], projectilePoolSize);
        }
    }

    private void Update()
    {
        UpdateProjectiles();
    }

    public Projectile[] SpawnBullets(Projectile projectilePrefab, ProjectileData projectileData, Transform[] firePositions, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        Projectile[] spawnedBullets = new Projectile[firePositions.Length];
        for (int i = 0; i < firePositions.Length; i++)
        {
            spawnedBullets[i] = SpawnBullet(projectilePrefab, projectileData, firePositions, i, relativeVelocity, useRelativeBulletSpeed);
        }

        return spawnedBullets;
    }

    public Projectile SpawnBullet(Projectile projectilePrefab, ProjectileData projectileData, Transform[] firePositions, int fireIndex = 0, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        Projectile spawnedBullet = ObjectPoolManager.Spawn(
            projectilePrefab,
            firePositions[fireIndex].position,
            Quaternion.identity
        );

        spawnedBullet.transform.right = firePositions[fireIndex].right;
        InitBullet(spawnedBullet, projectileData, relativeVelocity, useRelativeBulletSpeed);

        return spawnedBullet;
    }

    private void InitBullet(Projectile projectile, ProjectileData projectileData, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        projectile.projectileData = projectileData;

        if (useRelativeBulletSpeed)
        {
            relativeVelocity = Mathf.Max(relativeVelocity, 0);
            //projectile.projectileData.speed += relativeVelocity;
        }

        currentProjectiles.Add(projectile);
    }

    public void RemoveProjectileFromList(Projectile projectile)
    {
        currentProjectiles.Remove(projectile);
    }

    private void UpdateProjectiles()
    {
        currentProjectiles.RemoveAll(x => !x.gameObject.activeSelf);

        foreach (Projectile projectile in currentProjectiles)
        {
            if (projectile == null || !projectile.gameObject.activeSelf)
            {
                continue;
            }

            if (projectile.target != null)
            {         
                // Rotate projectile towards target
                Vector2 dirToTarget = (projectile.target.position - projectile.transform.position).normalized;
                Quaternion targetRot = Quaternion.Euler(0, 0, MKUtility.CalcDir2D(dirToTarget));
                projectile.transform.rotation = Quaternion.Lerp(projectile.transform.rotation, targetRot, Time.deltaTime * projectile.projectileData.rotationSpeed);
            }

            if (projectile.projectileData.speed != 0)
            {
                projectile.transform.Translate(Vector2.right * projectile.projectileData.speed * Time.deltaTime);
            }
        }
    }
}
