using UnityEngine;
using UnityEngine.UI;

namespace MK.UI
{
    public class UIPromptDisplay : MonoBehaviour
    {
        [Space]
        public LayoutGroup layoutGroup;
        public RectTransform rootRectTransform;
        public UITransitioner transitioner;

        [Header("Prefabs")]
        [SerializeField] Image standardImagePrefab;
        [SerializeField] Text standardTextPrefab;

        bool hasBeenInitialized;

        /// <summary>
        /// Inits the prompt.
        /// </summary>
        /// <param name="pos">Position.</param>
        /// <param name="lifetime">Life time.</param>
        /// <param name="elements">UI <paramref name="elements"/>. Can be <c>string</c>, <c>Sprite</c> or <c>GameObject</c>.</param>
        public void InitPrompt(Vector2 pos, TransitionPreset transitionPreset, float lifetime, params object[] elements)
        {
            hasBeenInitialized = false;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] is Sprite)
                {
                    Image spawnedImage = Instantiate(standardImagePrefab, layoutGroup.transform);
                    spawnedImage.sprite = elements[i] as Sprite;
                    spawnedImage.gameObject.SetActive(true);
                }
                else if (elements[i] is string)
                {
                    Text spawnedText = Instantiate(standardTextPrefab, transform);
                    spawnedText.text = elements[i] as string;
                    spawnedText.gameObject.SetActive(true);

                    ContentSizeFitter csf = spawnedText.GetComponent<ContentSizeFitter>();
                    csf.SetLayoutHorizontal();
                    Destroy(csf);

                    spawnedText.transform.SetParent(layoutGroup.transform);
                }
                else if (elements[i] is GameObject)
                {
                    GameObject spawnedGameObject = Instantiate(elements[i] as GameObject, layoutGroup.transform);
                }
                else
                {
                    Debug.LogError("An object was given to the UIPromptDisplay that could not be handled. (Param " + i + ").", this);
                    Destroy(gameObject);
                    return;
                }
            }

            rootRectTransform.anchoredPosition = pos;

            // if the lifetime is < 0f, the object is destroyed immediately
            // feel free to fix this problem inside of UITransition instead. /Tomas
            bool destroyAfterLifetime = lifetime >= 0f;
            transitioner.InitTransition(lifetime, destroyAfterLifetime, new UITransitionData(transitionPreset));

            hasBeenInitialized = true;
        }
    }
}