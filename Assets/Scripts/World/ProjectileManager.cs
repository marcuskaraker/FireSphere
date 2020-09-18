using MK;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private List<Projectile> currentProjectiles;

    private void Awake()
    {
        currentProjectiles = new List<Projectile>();
    }

    private void Update()
    {
        UpdateProjectiles();
    }

    public Projectile[] SpawnBullets(Projectile projectilePrefab, Transform[] firePositions, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true, ProjectileData projectileData = null)
    {
        Projectile[] spawnedBullets = new Projectile[firePositions.Length];
        for (int i = 0; i < firePositions.Length; i++)
        {
            spawnedBullets[i] = SpawnBullet(projectilePrefab, firePositions, i, relativeVelocity, useRelativeBulletSpeed, projectileData);
        }

        return spawnedBullets;
    }

    public Projectile SpawnBullet(Projectile projectilePrefab, Transform[] firePositions, int fireIndex = 0, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true, ProjectileData projectileData = null)
    {
        Projectile spawnedBullet = Instantiate(projectilePrefab, firePositions[fireIndex].position, Quaternion.identity);
        spawnedBullet.transform.right = firePositions[fireIndex].right;
        InitBullet(spawnedBullet, relativeVelocity, useRelativeBulletSpeed, projectileData);

        return spawnedBullet;
    }

    private void InitBullet(Projectile projectile, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true, ProjectileData projectileData = null)
    {
        if (useRelativeBulletSpeed)
        {
            relativeVelocity = Mathf.Max(relativeVelocity, 0);
            projectile.projectileData.speed += relativeVelocity;
        }

        if (projectileData != null)
        {
            projectile.projectileData = projectileData;
        }

        currentProjectiles.Add(projectile);
    }

    private void UpdateProjectiles()
    {
        if (Time.frameCount % 2 == 0)
        {
            currentProjectiles.RemoveAll(x => x == null);
        }

        foreach (Projectile projectile in currentProjectiles)
        {
            if (projectile == null)
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
