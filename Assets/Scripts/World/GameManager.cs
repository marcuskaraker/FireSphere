using UnityEngine;
using MK;
using System.Collections.Generic;
using System.Collections;
using MK.Destructible;

public enum GameState { None, IsRunning, GameOverSequence, MainMenu, HighScore }

public class GameManager : MonoBehaviorSingleton<GameManager>
{
    public GameData gameData;

    public string playerName = "Player0";
    public int killCounter;
    public BoxCollider2D arenaBounds;

    public PlayerInput PlayerInput { get; private set; }
    public GameObject Player { get; private set; }
    public UIManager UIManager { get; private set; }

    private List<Projectile> currentProjectiles;
    private Vector2 playerPosLastFrame;
    private SpaceBackgroundScroller uvScroller;

    private List<GameObject> worldObjects;

    public CameraController CameraController { get; private set; }
    public GameState GameState { get; private set; }

    public int[] HighScores { get; private set; }
    public string[] HighScoreNames { get; private set; }

    private const int HIGHSCORE_LIST_COUNT = 10;

    private void Awake()
    {
        RegisterSingleton();

        CameraController = FindObjectOfType<CameraController>();
        PlayerInput = FindObjectOfType<PlayerInput>();
        UIManager = FindObjectOfType<UIManager>();
       
        uvScroller = FindObjectOfType<SpaceBackgroundScroller>();

        currentProjectiles = new List<Projectile>();
        worldObjects = new List<GameObject>();

        HighScores = new int[HIGHSCORE_LIST_COUNT];
        HighScoreNames = new string[HIGHSCORE_LIST_COUNT];

        GameState = GameState.MainMenu;
        UIManager.SetMenuActive(GameState);
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

        if (GameState != GameState.IsRunning)
        {
            uvScroller.Scroll(Vector2.up * Time.deltaTime);
        }
    }

    private void OnDisable()
    {
        GameState = GameState.None;
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
                worldObjects.Add(
                    Instantiate(gameData.worldObjects[Random.Range(0, gameData.worldObjects.Length)], pos, Quaternion.identity)
                );
            }
        }
    }

    private IEnumerator DoSpawnEnemyCruisers()
    {
        while (GameState == GameState.IsRunning)
        {
            yield return new WaitForSeconds(Random.Range(gameData.cruiserMinMaxSpawnInterval.min, gameData.cruiserMinMaxSpawnInterval.max));
            SpawnCruiserAtRandomPos();
        }
    }

    private void SpawnCruiserAtRandomPos()
    {
        Vector2 pos = MKUtility.GetRandomPositionInBounds(arenaBounds.bounds);
        Instantiate(gameData.cruiserPrefab, pos, Quaternion.identity);

        UIManager.PromptIfEmpty(2f, MK.UI.TransitionPreset.ScaleIn, "An enemy cruiser has arrived!");
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
        if (GameState != GameState.IsRunning) return;

        killCounter++;
    }

    public void StartGame()
    {

        // Init Player
        SpawnPlayer();

        // Init World
        SpawnWorldObjects();

        GameState = GameState.IsRunning;
        
        // Enable Game UI and close menu.
        UIManager.SetMenuActive(GameState);

        SpawnCruiserAtRandomPos();
        StartCoroutine(DoSpawnEnemyCruisers());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SpawnPlayer()
    {
        Player = Instantiate(gameData.playerPrefab, Vector2.zero, Quaternion.identity);
        Shield playerShield = Instantiate(gameData.playerShieldPrefab, Vector2.zero, Quaternion.identity);

        Destructible playerDestructible = Player.GetComponent<Destructible>();
        playerShield.Target = playerDestructible;
        playerDestructible.onDeath.AddListener(delegate { GameOver(); });

        PlayerInput.ControlTarget = Player;
        PlayerInput.shieldEffect = playerShield;

        playerPosLastFrame = Player.transform.position;

        CameraController.Target = Player;
        uvScroller.Target = Player.transform;

        UIManager.AssignPlayerReference();
    }

    public void GameOver()
    {
        if (GameState != GameState.GameOverSequence)
        {
            StartCoroutine(DoGameOver());
        }     
    }

    private IEnumerator DoGameOver()
    {
        float gameOverTextTime = 2f;
        float killCounterTextTime = 3f;
        float highscoreTextTime = 2f;

        GameState = GameState.GameOverSequence;

        // Game over text
        UIManager.Prompt(gameOverTextTime, MK.UI.TransitionPreset.LeftToRight, "Game Over!");
        yield return new WaitForSeconds(gameOverTextTime);

        // Show kill count
        UIManager.Prompt(killCounterTextTime, MK.UI.TransitionPreset.LeftToRight, "You got " + killCounter + " kills!");
        yield return new WaitForSeconds(killCounterTextTime);

        // Save score
        int newHighScorePlace = SaveNewScore(killCounter);
        if (newHighScorePlace >= 0)
        {
            UIManager.Prompt(highscoreTextTime, MK.UI.TransitionPreset.LeftToRight, "New Highscore Placing! (" + (newHighScorePlace + 1) + ")");
            yield return new WaitForSeconds(highscoreTextTime);
        }

        ReturnToMainMenu(true);
    }

    public void ReturnToMainMenu()
    {
        ReturnToMainMenu(false);
    }

    public void ReturnToMainMenu(bool clearGameWorld = false)
    {
        if (GameState == GameState.MainMenu) return;

        if (clearGameWorld || GameState == GameState.IsRunning)
        {
            ClearGameWorld();
        }

        GameState = GameState.MainMenu;
        UIManager.SetMenuActive(GameState);
    }

    public void OpenHighscoreScreen()
    {
        if (GameState != GameState.MainMenu) return;

        int[] highscores = GetHighScores();
        for (int i = 0; i < highscores.Length; i++)
        {
            Debug.Log(i + ": " + highscores[i]);
        }

        GameState = GameState.HighScore;
        UIManager.SetMenuActive(GameState);
    }

    private void ClearGameWorld()
    {
        foreach (GameObject worldObject in worldObjects)
        {
            Destroy(worldObject);
        }

        worldObjects.Clear();

        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            Destroy(enemy.gameObject);
        }

        if (Player != null)
        {
            Destroy(Player);
        }

        killCounter = 0;
        uvScroller.ResetPos();
        CameraController.ResetPos();
    }

    public int[] GetHighScores()
    {
        int[] highscores = new int[HIGHSCORE_LIST_COUNT];
        for (int i = 0; i < highscores.Length; i++)
        {
            highscores[i] = PlayerPrefs.GetInt(i.ToString(), 0);
            HighScoreNames[i] = PlayerPrefs.GetString(i.ToString(), "XXX");
        }

        HighScores = highscores;
        return highscores;
    }

    public void SetHighScores(int[] highscores)
    {
        for (int i = 0; i < highscores.Length; i++)
        {
            PlayerPrefs.SetInt(i.ToString(), highscores[i]);
            PlayerPrefs.SetString(i.ToString(), HighScoreNames[i]);
        }

        HighScores = highscores;
    }

    public int SaveNewScore(int score)
    {
        int[] highscores = GetHighScores();

        for (int i = 0; i < highscores.Length; i++)
        {
            if (score > highscores[i])
            {
                highscores[i] = score;
                HighScoreNames[i] = playerName;
                SetHighScores(highscores);
                return i;
            }
        }
       
        return -1;
    }
}
