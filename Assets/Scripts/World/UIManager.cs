using UnityEngine;
using UnityEngine.UI;
using MK.Destructible;
using MK.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerController;

    public Canvas mainCanvas;
    public Transform promptParent;
    public HealthBar healthBar;
    public Text killCounterText;
    public UIPromptDisplay promptDisplayPrefab;

    private Destructible playerDestructible;

    private UIPromptDisplay currentlySpawnedPrompt;

    private void Awake()
    {
        Cursor.visible = false;

        playerDestructible = playerController.controlTarget.GetComponent<Destructible>();
    }

    private void Update()
    {
        healthBar.SetValue(
            playerDestructible.health / playerDestructible.maxHealth, 
            "HP: " + Mathf.RoundToInt(playerDestructible.health) + " / " + Mathf.RoundToInt(playerDestructible.maxHealth)
        );
    }

    public UIPromptDisplay Prompt(string text, float lifetime = 1f)
    {
        UIPromptDisplay spawnedPrompt = Instantiate(promptDisplayPrefab, Vector2.zero, Quaternion.identity, promptParent);
        spawnedPrompt.InitPrompt(Vector2.zero, TransitionPreset.ScaleIn, lifetime, text);

        return spawnedPrompt;
    }

    public void PromptIfEmpty(string text, float lifetime = 1f)
    {
        if (currentlySpawnedPrompt != null) return;

        currentlySpawnedPrompt = Prompt(text, lifetime);
    }

    public void SetKillCounterText(string text)
    {
        killCounterText.text = text;
    }
}
