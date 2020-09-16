﻿using UnityEngine;
using UnityEngine.UI;
using MK.Destructible;
using MK.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerController;

    public Canvas mainCanvas;
    public Transform promptParent;
    public Text killCounterText;
    public UIPromptDisplay promptDisplayPrefab;

    [Header("Loadout")]
    public LayoutGroup loadoutLayout;
    public SelectionSlot selectionSlotPrefab;
    private List<SelectionSlot> loadOutSlots;

    [Header("Bars")]
    public HealthBar healthBar;
    public HealthBar shieldBar;

    private Destructible playerDestructible;
    private UIPromptDisplay currentlySpawnedPrompt;

    [Header("Menus")]
    public GameObject gameUIParent;
    public GameObject mainMenuParent;
    public GameObject highscoreMenuParent;

    [Space]
    public LayoutGroup mainMenuButtunLayout;
    public LayoutGroup highScoreLayout;

    [Space]
    public Text highscoreTextPrefab;

    private List<Text> highscoreTextList;

    private const float MAIN_MENU_BUTTON_ENABLE_TIME_INTERVAL = 0.1f;

    private void Awake()
    {
        Cursor.visible = false;
        loadOutSlots = new List<SelectionSlot>();
        highscoreTextList = new List<Text>();
    }

    private void Start()
    {
        InitHighScoreList();
    }

    private void Update()
    {
        if (playerDestructible == null)
        {
            return;
        }

        // Healthbar
        UpdatePlayerHealthBar();

        // Shield
        UpdatePlayerShieldBar();

        // Loadout
        UpdateLoadoutLayout();
    }

    public UIPromptDisplay Prompt(float lifetime, TransitionPreset transitionPreset, params object[] elements)
    {
        UIPromptDisplay spawnedPrompt = Instantiate(promptDisplayPrefab, Vector2.zero, Quaternion.identity, promptParent);
        spawnedPrompt.InitPrompt(Vector2.zero, transitionPreset, lifetime, elements);

        return spawnedPrompt;
    }

    public void PromptIfEmpty(float lifetime, TransitionPreset transitionPreset, params object[] elements)
    {
        if (currentlySpawnedPrompt != null) return;

        currentlySpawnedPrompt = Prompt(lifetime, transitionPreset, elements);
    }

    public void SetKillCounterText(string text)
    {
        killCounterText.text = text;
    }

    public void UpdatePlayerHealthBar()
    {
        healthBar.SetValue(
            playerDestructible.health / playerDestructible.maxHealth,
            "HP: " + Mathf.RoundToInt(playerDestructible.health) + " / " + Mathf.RoundToInt(playerDestructible.maxHealth)
        );
    }

    public void UpdatePlayerShieldBar()
    {
        float shieldDisplayValue = 1f;
        shieldBar.Slider.interactable = true;
        if (playerController.shieldEffect.ShieldState == ShieldState.Open)
        {
            shieldDisplayValue = playerController.shieldEffect.ShieldTimer / playerController.shieldEffect.shieldDuration;
        }
        else if (playerController.shieldEffect.ShieldState == ShieldState.Closing)
        {
            shieldDisplayValue = playerController.shieldEffect.ShieldTimer / playerController.shieldEffect.shieldCooldown;
            shieldBar.Slider.interactable = false;
        }

        shieldBar.SetValue(
             shieldDisplayValue,
             "Shield: " + Mathf.RoundToInt(shieldDisplayValue * 100f) + "%"
        );
    }

    public void UpdateLoadoutLayout()
    {
        // Spawn and remove slots.
        if (loadoutLayout.transform.childCount < playerController.Shooter.loadout.Length)
        {
            // There are too few UI slots, spawn some more.
            for (int i = loadoutLayout.transform.childCount; i < playerController.Shooter.loadout.Length; i++)
            {
                loadOutSlots.Add(SpawnLoadoutSlot());
            }
        }
        else if (loadoutLayout.transform.childCount > playerController.Shooter.loadout.Length)
        {
            // There are too many UI slots, remove some.
            for (int i = loadoutLayout.transform.childCount; i > playerController.Shooter.loadout.Length; i--)
            {
                Destroy(loadOutSlots[i].gameObject);
                loadOutSlots.RemoveAt(i);
            }
        }

        // Set slot icon and selection
        for (int i = 0; i < loadoutLayout.transform.childCount; i++)
        {
            bool isSelected = i == playerController.Shooter.CurrentWeaponIndex;
            loadOutSlots[i].Select(isSelected);

            Weapon weapon = playerController.Shooter.loadout[i];
            loadOutSlots[i].SetIcon(
                weapon != null ? weapon.weaponIcon : null, 
                weapon != null ? weapon.weaponIconColor : Color.white
            );

            // Durability
            if (playerController.Shooter.durability[i] >= 0)
            {
                float durabilityPercentage = playerController.Shooter.GetDurabilityPercentageOfWeapon(i);
                loadOutSlots[i].durabilityBar.SetValue(durabilityPercentage);
                loadOutSlots[i].durabilityBar.Text.text = "";
            }
            else
            {
                loadOutSlots[i].durabilityBar.SetValue(1);
                loadOutSlots[i].durabilityBar.Text.text = "Infinite";
            }          
        }
    }

    private SelectionSlot SpawnLoadoutSlot()
    {
        return Instantiate(selectionSlotPrefab, loadoutLayout.transform);
    }

    public void SetMenuActive(GameState gameState)
    {
        gameUIParent.SetActive(false);
        mainMenuParent.SetActive(false);
        highscoreMenuParent.SetActive(false);

        switch (gameState)
        {
            case GameState.IsRunning:
                gameUIParent.SetActive(true);
                break;
            case GameState.MainMenu:
                mainMenuParent.SetActive(true);
                break;
            case GameState.HighScore:
                highscoreMenuParent.SetActive(true);
                UpdateHighScoreList();
                break;
        }

        StartCoroutine(DoEnableMainMenuButtons(gameState == GameState.MainMenu, MAIN_MENU_BUTTON_ENABLE_TIME_INTERVAL));
    }

    public void InitHighScoreList()
    {
        for (int i = 0; i < GameManager.Instance.HighScores.Length; i++)
        {
            Text spawnedHighscoreText = Instantiate(highscoreTextPrefab, highScoreLayout.transform);
            spawnedHighscoreText.text = GetHighscoreText(i, 0, "XXX");
            highscoreTextList.Add(spawnedHighscoreText);
        }
    }

    public void UpdateHighScoreList()
    {
        for (int i = 0; i > highscoreTextList.Count; i++)
        {
            highscoreTextList[i].text = GetHighscoreText(
                i,
                GameManager.Instance.HighScores[i],
                GameManager.Instance.HighScoreNames[i]
            );
        }
    }

    private IEnumerator DoEnableMainMenuButtons(bool value, float waitTime)
    {
        for (int i = 0; i < mainMenuButtunLayout.transform.childCount; i++)
        {
            yield return new WaitForSeconds(waitTime);
            Button button = mainMenuButtunLayout.transform.GetChild(i).GetComponent<Button>();
            button.interactable = value;
        }
    }

    private string GetHighscoreText(int placing, int score, string name)
    {
        return (placing+1) + ". " + name + " - " + score;
    }

    public void AssignPlayerReference()
    {
        playerDestructible = playerController.ControlTarget.GetComponent<Destructible>();
    }
}
