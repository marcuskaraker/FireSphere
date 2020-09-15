using UnityEngine;
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

    private void Awake()
    {
        Cursor.visible = false;

        playerDestructible = playerController.controlTarget.GetComponent<Destructible>();
        loadOutSlots = new List<SelectionSlot>();
    }

    private void Update()
    {
        // Healthbar
        healthBar.SetValue(
            playerDestructible.health / playerDestructible.maxHealth, 
            "HP: " + Mathf.RoundToInt(playerDestructible.health) + " / " + Mathf.RoundToInt(playerDestructible.maxHealth)
        );

        // Shield
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

        // Loadout
        UpdateLoadoutLayout();
    }

    public UIPromptDisplay Prompt(float lifetime, params object[] elements)
    {
        UIPromptDisplay spawnedPrompt = Instantiate(promptDisplayPrefab, Vector2.zero, Quaternion.identity, promptParent);
        spawnedPrompt.InitPrompt(Vector2.zero, TransitionPreset.ScaleIn, lifetime, elements);

        return spawnedPrompt;
    }

    public void PromptIfEmpty(float lifetime, params object[] elements)
    {
        if (currentlySpawnedPrompt != null) return;

        currentlySpawnedPrompt = Prompt(lifetime, elements);
    }

    public void SetKillCounterText(string text)
    {
        killCounterText.text = text;
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
}
