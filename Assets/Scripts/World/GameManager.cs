using UnityEngine;
using MK;
using System.Collections.Generic;

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    public PlayerInput PlayerInput { get; private set; }
    public GameObject Player { get; private set; }
    public UIManager UIManager { get; private set; }

    List<Projectile> currentProjectiles;

    Vector2 playerPosLastFrame;

    SpaceBackgroundScroller uvScroller;

    private void Awake()
    {
        RegisterSingleton();
        PlayerInput = FindObjectOfType<PlayerInput>();
        UIManager = FindObjectOfType<UIManager>();
        Player = PlayerInput.controlTarget.gameObject;

        uvScroller = FindObjectOfType<SpaceBackgroundScroller>();

        currentProjectiles = new List<Projectile>();

        playerPosLastFrame = Player.transform.position;
    }

    private void Update()
    {
        UpdateProjectiles();

        if (Player != null)
        {
            Vector2 playerDeltaPos = ((Vector2)Player.transform.position - playerPosLastFrame);
            uvScroller.Scroll(playerDeltaPos);
            playerPosLastFrame = Player.transform.position;
        }
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

            if (projectile.speed != 0)
            {
                // TODO: Shouldn't this need to be projectile.transform.right? Why does the global Vector2.right work here?
                projectile.transform.Translate(Vector2.right * projectile.speed * Time.deltaTime);
            }
        }
    }

    public Projectile[] SpawnBullets(Projectile projectilePrefab, Transform[] firePositions, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        Projectile[] spawnedBullets = new Projectile[firePositions.Length];
        for (int i = 0; i < firePositions.Length; i++)
        {
            spawnedBullets[i] = SpawnBullet(projectilePrefab, firePositions, i, relativeVelocity, useRelativeBulletSpeed);
        }

        return spawnedBullets;
    }

    public Projectile SpawnBullet(Projectile projectilePrefab, Transform[] firePositions, int fireIndex = 0, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        Projectile spawnedBullet = Instantiate(projectilePrefab, firePositions[fireIndex].position, Quaternion.identity);
        spawnedBullet.transform.right = firePositions[fireIndex].right;
        InitBullet(spawnedBullet, relativeVelocity, useRelativeBulletSpeed);

        return spawnedBullet;
    }

    protected void InitBullet(Projectile projectile, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true)
    {
        if (useRelativeBulletSpeed)
        {
            relativeVelocity = Mathf.Max(relativeVelocity, 0);
            projectile.speed += relativeVelocity;
        }

        currentProjectiles.Add(projectile);
    }
}
