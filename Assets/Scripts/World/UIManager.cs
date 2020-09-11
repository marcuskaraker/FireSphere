using UnityEngine;
using MK.Destructible;
using MK.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public PlayerInput playerController;

    public Canvas mainCanvas;
    public HealthBar healthBar;
    public UIPromptDisplay promptDisplayPrefab;

    Destructible playerDestructible;

    private void Awake()
    {
        Cursor.visible = false;

        playerDestructible = playerController.controlTarget.GetComponent<Destructible>();

        StartCoroutine(TestPrompt());
    }

    private void Update()
    {
        healthBar.SetValue(
            playerDestructible.health / playerDestructible.maxHealth, 
            "HP: " + Mathf.RoundToInt(playerDestructible.health) + " / " + Mathf.RoundToInt(playerDestructible.maxHealth)
        );
    }

    public void Prompt(string text)
    {
        UIPromptDisplay spawnedPrompt = Instantiate(promptDisplayPrefab, Vector2.zero, Quaternion.identity, mainCanvas.transform);
        spawnedPrompt.InitPrompt(Vector2.zero, TransitionPreset.ScaleIn, 2f, text);
    }

    IEnumerator TestPrompt()
    {
        yield return new WaitForSeconds(2f);
        Prompt("Testing!!");
    }
}
