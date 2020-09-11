using System.Collections;
using UnityEngine;

namespace MK.UI
{
    public class UIPromptDisplaySample : MonoBehaviour
    {
        // With UIPromptDisplay I decided to make the user in charge of keeping track of which prefab to spawn, instead of having a static call to some default prefab.
        // This way it is easy to oneself define which prompt to have and where.
        [SerializeField] UIPromptDisplay defaultPromptDisplay;

        // Spawning the UIPromptDisplay as a child to another UI object gives you much in-editor control of where the display should spawn.
        [SerializeField] Transform promptDisplayParent;

        // Sprites dispayed with UIPromptDisplay should be always be squares (x*x size).
        [SerializeField] Sprite testSprite;

        [Header("Sample Options")]
        [SerializeField] float lifeTime = 2f;
        [SerializeField] float spawnInterval = 3f;

        private void Start()
        {
            StartCoroutine(DoSpawnPromptAfterTime(spawnInterval));
        }

        private IEnumerator DoSpawnPromptAfterTime(float time)
        {
            for (int i = 0; i < 1000; i++)
            {
                yield return new WaitForSeconds(time);
                SpawnSamplePrompt((TransitionPreset)(i % 4), lifeTime);
            }
        }

        private void SpawnSamplePrompt(TransitionPreset preset, float lifeTime)
        {
            UIPromptDisplay spawnedPD = Instantiate(defaultPromptDisplay, promptDisplayParent);
            spawnedPD.InitPrompt(Vector2.zero, preset, lifeTime, testSprite, "Some cool sample text.");
        }
    }
}