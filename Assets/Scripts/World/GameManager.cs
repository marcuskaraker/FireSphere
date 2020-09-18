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

    private List<EnemyController> enemies;
    private List<Cruiser> cruisers;
    private Vector2 playerPosLastFrame;
    private SpaceBackgroundScroller uvScroller;

    private List<GameObject> worldObjects;

    // Player
    public PlayerInput PlayerInput { get; private set; }
    public GameObject Player { get; private set; }

    // Managers
    public UIManager UIManager { get; private set; }
    public ProjectileManager ProjectileManager { get; private set; }
    public CameraController CameraController { get; private set; }

    // Runtime
    public GameState GameState { get; private set; }
    public Cruiser ClosestCruiser { get; private set; }

    // Highscores
    public int[] HighScores { get; private set; }
    public string[] HighScoreNames { get; private set; }

    // Constants
    private const int HIGHSCORE_LIST_COUNT = 10;
    private const string HIGHSCORE_SERIALIZED_NUMBERKEY = "Highscore";
    private const string HIGHSCORE_SERIALIZED_NAMEKEY = "HighscoreName";

    private void Awake()
    {
        RegisterSingleton();

        CameraController = FindObjectOfType<CameraController>();
        PlayerInput = FindObjectOfType<PlayerInput>();
        UIManager = FindObjectOfType<UIManager>();
        ProjectileManager = FindObjectOfType<ProjectileManager>();

        uvScroller = FindObjectOfType<SpaceBackgroundScroller>();

        worldObjects = new List<GameObject>();
        cruisers = new List<Cruiser>();
        enemies = new List<EnemyController>();

        HighScores = new int[HIGHSCORE_LIST_COUNT];
        HighScoreNames = new string[HIGHSCORE_LIST_COUNT];

        LoadHighScores();

        GameState = GameState.MainMenu;
        UIManager.SetMenuActive(GameState);      
    }

    private void Update()
    {
        UpdateClosestCruiserToPlayer();
        UIManager.SetKillCounterText(killCounter.ToString());

        if (GameState != GameState.IsRunning)
        {
            uvScroller.Scroll(Vector2.up * Time.deltaTime);
        }

        // Debug Input
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
        {
            ClearHighscores();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
        {
            if (Player != null)
            {
                Player.GetComponent<Destructible>().Hurt(999);
            }          
        }
    }

    private void OnDisable()
    {
        GameState = GameState.None;
    }

    public void IncreaseKillCounter()
    {
        if (GameState != GameState.IsRunning) return;

        killCounter++;
    }

    public void UpdatePlayerName()
    {
        playerName = UIManager.nameInput.text;
    }

    #region Cruisers 
    private void UpdateClosestCruiserToPlayer()
    {
        if (Player != null)
        {
            Vector2 playerDeltaPos = ((Vector2)Player.transform.position - playerPosLastFrame);
            uvScroller.Scroll(playerDeltaPos);
            playerPosLastFrame = Player.transform.position;

            if (Time.frameCount % 10 == 0) // Run every tenth frame
            {
                ClosestCruiser = GetClosestCruiser(Player.transform.position);
                UIManager.objectiveArrow.target = ClosestCruiser != null ? ClosestCruiser.transform : null;
            }
        }
    }

    private IEnumerator DoSpawnEnemyCruisers()
    {
        while (GameState == GameState.IsRunning)
        {
            yield return new WaitForSeconds(Random.Range(gameData.cruiserMinMaxSpawnInterval.min, gameData.cruiserMinMaxSpawnInterval.max));
            if (GameState == GameState.IsRunning)
            {
                SpawnCruiserAtRandomPos();
            }          
        }
    }

    private void SpawnCruiserAtRandomPos()
    {
        cruisers.RemoveAll(x => x == null);

        Vector2 pos = MKUtility.GetRandomPositionInBounds(arenaBounds.bounds, gameData.spawningPadding);
        Cruiser cruiser = Instantiate(gameData.cruiserPrefab, pos, Quaternion.identity);
        cruisers.Add(cruiser);

        UIManager.PromptIfEmpty(2f, MK.UI.TransitionPreset.ScaleIn, "An enemy cruiser has arrived!");

        DestroyWorldObjectsAroundPos(pos, gameData.cruiserClearRadius);
    }

    public Cruiser GetClosestCruiser(Vector2 pos)
    {
        Cruiser closestCruiser = null;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < cruisers.Count; i++)
        {
            if (cruisers[i] == null) continue;

            float distance = Vector2.Distance(pos, (Vector2)cruisers[i].transform.position);
            if (distance < closestDistance)
            {
                closestCruiser = cruisers[i];
                closestDistance = distance;
            }
        }

        return closestCruiser;
    }
    #endregion

    #region Enemies
    public void SpawnEnemy(EnemyController enemyPrefab, Vector2 pos)
    {
        EnemyController spawnedEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

        if (Player != null)
        {
            spawnedEnemy.chaseTarget = Player.transform;
        }

        enemies.Add(spawnedEnemy);
    }

    public EnemyController GetClosestEnemyToPos(Vector2 pos)
    {
        bool enemyDeadFlag = false;

        EnemyController closestEnemy = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null) { enemyDeadFlag = true; continue; }

            float distance = Vector2.Distance(pos, enemies[i].transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = enemies[i];
                closestDistance = distance;
            }
        }

        if (enemyDeadFlag) enemies.RemoveAll(x => x == null);

        return closestEnemy;
    }

    #endregion

    #region Pickups 
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
    #endregion

    #region GeneralSpawning
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

        DestroyWorldObjectsAroundPos(Player.transform.position, gameData.playerClearRadius);
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

    private void ClearGameWorld()
    {
        // Destroy world objects
        foreach (GameObject worldObject in worldObjects)
        {
            Destroy(worldObject);
        }

        worldObjects.Clear();

        // Destroy Cruisers
        foreach (Cruiser cruiser in FindObjectsOfType<Cruiser>())
        {
            Destroy(cruiser.gameObject);
        }

        // Destroy Enemy Ships
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>())
        {
            Destroy(enemy.gameObject);
        }

        // Destroy Pickups
        foreach (PickupInteractable pickup in FindObjectsOfType<PickupInteractable>())
        {
            Destroy(pickup.gameObject);
        }

        if (Player != null)
        {
            Destroy(Player);
        }

        killCounter = 0;
        uvScroller.ResetPos();
        CameraController.ResetPos();
    }

    public void DestroyWorldObjectsAroundPos(Vector2 pos, float radius)
    {
        for (int i = 0; i < worldObjects.Count; i++)
        {
            if (worldObjects[i] == null) continue;

            float distance = Vector2.Distance(worldObjects[i].transform.position, pos);
            if (distance < radius)
            {
                Destroy(worldObjects[i]);
            }
        }

        worldObjects.RemoveAll(x => x == null);
    }
    #endregion

    #region StartAndEnd
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
    #endregion

    #region Menu
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
    #endregion

    #region HighScore
    public void OpenHighscoreScreen()
    {
        if (GameState != GameState.MainMenu) return;

        GameState = GameState.HighScore;
        UIManager.SetMenuActive(GameState);
    }

    public void LoadHighScores()
    {     
        HighScores = new int[HIGHSCORE_LIST_COUNT];
        for (int i = 0; i < HighScores.Length; i++)
        {
            HighScores[i] = PlayerPrefs.GetInt(HIGHSCORE_SERIALIZED_NUMBERKEY + i.ToString(), 0);
            HighScoreNames[i] = PlayerPrefs.GetString(HIGHSCORE_SERIALIZED_NAMEKEY + i.ToString(), "XXX");
        }
    }

    public void ClearHighscores()
    {
        for (int i = 0; i < HighScores.Length; i++)
        {
            PlayerPrefs.DeleteKey(HIGHSCORE_SERIALIZED_NUMBERKEY + i.ToString());
            PlayerPrefs.DeleteKey(HIGHSCORE_SERIALIZED_NAMEKEY + i.ToString());
        }
    }

    public void SaveHighScores()
    {
        for (int i = 0; i < HighScores.Length; i++)
        {
            PlayerPrefs.SetInt(HIGHSCORE_SERIALIZED_NUMBERKEY + i.ToString(), HighScores[i]);
            PlayerPrefs.SetString(HIGHSCORE_SERIALIZED_NAMEKEY + i.ToString(), HighScoreNames[i]);
        }

        PlayerPrefs.Save();
    }

    public int SaveNewScore(int score)
    {
        for (int i = 0; i < HighScores.Length; i++)
        {
            if (score > HighScores[i])
            {
                System.Array.Copy(HighScores, i, HighScores, i + 1, HighScores.Length - 1 - i);
                System.Array.Copy(HighScoreNames, i, HighScoreNames, i + 1, HighScoreNames.Length - 1 - i);
                HighScores[i] = score;
                HighScoreNames[i] = playerName;
                SaveHighScores();
                return i;
            }
        }
       
        return -1;
    }
    #endregion
}
