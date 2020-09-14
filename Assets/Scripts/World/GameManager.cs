using UnityEngine;
using MK;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    public GameData gameData;

    public int killCounter;
    public BoxCollider2D arenaBounds;

    public PlayerInput PlayerInput { get; private set; }
    public GameObject Player { get; private set; }
    public UIManager UIManager { get; private set; }

    private List<Projectile> currentProjectiles;
    private Vector2 playerPosLastFrame;
    private SpaceBackgroundScroller uvScroller;

    public bool GameIsRunning { get; private set; }

    private void Awake()
    {
        RegisterSingleton();
        PlayerInput = FindObjectOfType<PlayerInput>();
        UIManager = FindObjectOfType<UIManager>();
        Player = PlayerInput.controlTarget.gameObject;

        uvScroller = FindObjectOfType<SpaceBackgroundScroller>();

        currentProjectiles = new List<Projectile>();

        playerPosLastFrame = Player.transform.position;

        SpawnWorldObjects();

        GameIsRunning = true;

        StartCoroutine(DoSpawnEnemyCruisers());
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

        UIManager.SetKillCounterText(killCounter.ToString());
    }

    private void OnDisable()
    {
        GameIsRunning = false;
    }

    private void SpawnWorldObjects()
    {
        for (int x = 0; x < arenaBounds.bounds.size.x; x++)
        {
            for (int y = 0; y < arenaBounds.bounds.size.y; y++)
            {
                if (Random.Range(0f, 1f) > gameData.worldObjectSpawnChance) continue;

                Vector2 randomOffset = Random.insideUnitCircle;
                Vector2 pos = (new Vector2(x, y) + randomOffset) - (Vector2)arenaBounds.bounds.extents;
                Instantiate(gameData.worldObjects[Random.Range(0, gameData.worldObjects.Length)], pos, Quaternion.identity);
            }
        }
    }

    private IEnumerator DoSpawnEnemyCruisers()
    {
        while (GameIsRunning)
        {
            yield return new WaitForSeconds(Random.Range(gameData.cruiserMinMaxSpawnInterval.min, gameData.cruiserMinMaxSpawnInterval.max));

            Vector2 pos = MKUtility.GetRandomPositionInBounds(arenaBounds.bounds);
            Instantiate(gameData.cruiserPrefab, pos, Quaternion.identity);

            UIManager.PromptIfEmpty("An enemy cruiser has arrived!", 2f);
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

            if (projectile.projectileData.speed != 0)
            {
                // TODO: Shouldn't this need to be projectile.transform.right? Why does the global Vector2.right work here?
                projectile.transform.Translate(Vector2.right * projectile.projectileData.speed * Time.deltaTime);
            }
        }
    }

    public PickupInteractable SpawnPickup(string name, Vector2 pos)
    {
        Pickup pickupToSpawn = gameData.GetPickup(name);
        return SpawnPickup(pickupToSpawn, pos);
    }

    public PickupInteractable SpawnPickup(Pickup pickup, Vector2 pos)
    {
        if (pickup != null)
        {
            PickupInteractable pickupInteractable = Instantiate(gameData.pickupPrefab, pos, Quaternion.identity);
            pickupInteractable.pickupData = pickup;
            pickupInteractable.SetIcon(pickup.icon, pickup.iconColor);

            return pickupInteractable;
        }

        return null;
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

    protected void InitBullet(Projectile projectile, float relativeVelocity = 0f, bool useRelativeBulletSpeed = true, ProjectileData projectileData = null)
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

    public void IncreaseKillCounter()
    {
        killCounter++;
    }
}
