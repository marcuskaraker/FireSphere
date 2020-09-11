using UnityEngine;

namespace MK.UI
{
    public enum TransitionPreset { FadeIn, ScaleIn, TopDown, LeftToRight, None }

    [RequireComponent(typeof(CanvasGroup))]
    public class UITransitioner : MonoBehaviour
    {
        [Tooltip("The lifetime of the transition. Together with the speed variable, this decides how long the ui element is shown on screen. A negative value means infinite lifetime.")]
        public float lifetime = -1f;
        public float LifeTimer { get; private set; }

        public bool destroyAfterLifetime = true;

        [Tooltip("If this is checked, the transitioner will initialise itself on awake with the values specified in the inspector.")]
        [SerializeField] bool initOnAwake;

        public UITransitionData uiTransitionData;

        bool hasBeenInitialized;
        CanvasGroup canvasGroup;
        Vector2 basePosition;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            if (initOnAwake)
            {
                InitTransition(lifetime, destroyAfterLifetime, uiTransitionData);
            }
        }

        /// <summary>
        /// Inits the transition with the given values. A transition will not start before initialisation.
        /// </summary>
        /// <param name="lifetime">Lifetime.</param>
        /// <param name="destroyAfterLifetime">If set to <c>true</c> destroy after lifetime.</param>
        /// <param name="uiTransitionData">User interface transition data.</param>
        public void InitTransition(float lifetime, bool destroyAfterLifetime, UITransitionData uiTransitionData)
        {
            hasBeenInitialized = false;

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            this.lifetime = lifetime;
            this.destroyAfterLifetime = destroyAfterLifetime;
            this.uiTransitionData = uiTransitionData;

            basePosition = uiTransitionData.localPosition ? transform.localPosition : transform.position;

            // Init zero alpha.
            if (uiTransitionData.alphaFadeIn)
            {
                canvasGroup.alpha = 0f;
            }

            // Init zero scale.
            if (uiTransitionData.scaleFadeIn)
            {
                transform.localScale = Vector3.zero;
            }

            // Init flag pos;
            if (uiTransitionData.positionFadeIn)
            {
                if (uiTransitionData.localPosition)
                {
                    transform.localPosition = uiTransitionData.startPosition;
                }
                else
                {
                    transform.position = uiTransitionData.startPosition;
                }
            }

            if (destroyAfterLifetime)
            {
                Destroy(gameObject, lifetime + 0.1f);
            }

            hasBeenInitialized = true;
        }

        private void Update()
        {
            if (hasBeenInitialized)
            {
                LifeTimer += Time.deltaTime;

                //////////////////////
                // ALPHA TRANSITION //
                //////////////////////
                if (uiTransitionData.alphaFadeIn && (LifeTimer < (lifetime - (1 / uiTransitionData.alphaSpeed)) || lifetime < 0))
                {
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, Time.deltaTime * uiTransitionData.alphaSpeed);
                }
                else if (uiTransitionData.alphaFadeOut && LifeTimer > (lifetime - (1 / uiTransitionData.alphaSpeed)))
                {
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime * uiTransitionData.alphaSpeed);
                }

                //////////////////////
                // SCALE TRANSITION //
                //////////////////////
                if (uiTransitionData.scaleFadeIn && (LifeTimer < (lifetime - (1 / uiTransitionData.scaleSpeed)) || lifetime < 0))
                {
                    transform.localScale = Vector3.Lerp(
                        transform.localScale,
                        Vector3.one,
                        Time.deltaTime * uiTransitionData.scaleSpeed
                    );
                }
                else if (uiTransitionData.scaleFadeOut && LifeTimer > (lifetime - (1 / uiTransitionData.scaleSpeed)))
                {
                    transform.localScale = Vector3.Lerp(
                        transform.localScale,
                        Vector3.zero,
                        Time.deltaTime * uiTransitionData.scaleSpeed
                    );
                }

                /////////////////////////
                // POSITION TRANSITION //
                /////////////////////////
                if (uiTransitionData.positionFadeIn && (LifeTimer < (lifetime - (1 / uiTransitionData.moveSpeed)) || lifetime < 0))
                {
                    if (uiTransitionData.localPosition)
                    {
                        transform.localPosition = Vector3.Lerp(
                            transform.localPosition,
                            basePosition,
                            Time.deltaTime * uiTransitionData.moveSpeed
                        );
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(
                            transform.position,
                            basePosition,
                            Time.deltaTime * uiTransitionData.moveSpeed
                        );
                    }
                }
                else if (uiTransitionData.positionFadeOut && LifeTimer > (lifetime - (1 / uiTransitionData.moveSpeed)))
                {
                    if (uiTransitionData.localPosition)
                    {
                        transform.localPosition = Vector3.Lerp(
                            transform.localPosition,
                            uiTransitionData.endPosition,
                            Time.deltaTime * uiTransitionData.moveSpeed
                        );
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(
                            transform.position,
                            uiTransitionData.endPosition,
                            Time.deltaTime * uiTransitionData.moveSpeed
                        );
                    }
                }
            }
        }
    }

    /// <summary>
    /// A struct containing all required data for a UI Transition.
    /// </summary>
    [System.Serializable]
    public struct UITransitionData
    {
        [Header("Alpha")]
        public bool useAlpha;
        public float alphaSpeed;

        [Space]
        public bool alphaFadeIn;
        public bool alphaFadeOut;

        [Header("Scale")]
        public bool useScale;
        public float scaleSpeed;

        [Space]
        public bool scaleFadeIn;
        public bool scaleFadeOut;

        [Header("Positon")]
        public bool usePosition;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public bool localPosition;
        public float moveSpeed;

        [Space]
        public bool positionFadeIn;
        public bool positionFadeOut;

        public UITransitionData(bool globalTrueValues, bool useAlpha, bool useScale, bool usePosition = false)
        {
            this.useAlpha = useAlpha;
            this.useScale = useScale;
            this.usePosition = usePosition;

            alphaSpeed = 10f;
            scaleSpeed = 10f;
            moveSpeed = 10f;

            alphaFadeIn = globalTrueValues;
            alphaFadeOut = globalTrueValues;
            scaleFadeIn = globalTrueValues;
            scaleFadeOut = globalTrueValues;
            positionFadeIn = globalTrueValues;
            positionFadeOut = globalTrueValues;
            localPosition = globalTrueValues;

            startPosition = new Vector2(-2000f, 0f);
            endPosition = new Vector2(2000f, 0f);
        }

        public UITransitionData(bool useAlpha, bool useScale, bool usePosition, float alphaSpeed, float scaleSpeed, float moveSpeed, bool alphaFadeIn, bool alphaFadeOut,
                                bool scaleFadeIn, bool scaleFadeOut, bool positionFadeIn, bool positionFadeOut, bool localPosition, Vector2 startPosition, Vector2 endPosition)
        {
            this.useAlpha = useAlpha;
            this.useScale = useScale;
            this.usePosition = usePosition;

            this.alphaSpeed = alphaSpeed;
            this.scaleSpeed = scaleSpeed;
            this.moveSpeed = moveSpeed;

            this.alphaFadeIn = alphaFadeIn;
            this.alphaFadeOut = alphaFadeOut;
            this.scaleFadeIn = scaleFadeIn;
            this.scaleFadeOut = scaleFadeOut;
            this.positionFadeIn = positionFadeIn;
            this.positionFadeOut = positionFadeOut;
            this.localPosition = localPosition;

            this.startPosition = startPosition;
            this.endPosition = endPosition;
        }

        public UITransitionData(TransitionPreset transitionPreset)
        {
            useAlpha = false;
            useScale = false;
            usePosition = false;

            alphaSpeed = 10f;
            scaleSpeed = 10f;
            moveSpeed = 10f;

            alphaFadeIn = false;
            alphaFadeOut = false;
            scaleFadeIn = false;
            scaleFadeOut = false;
            positionFadeIn = false;
            positionFadeOut = false;
            localPosition = false;

            startPosition = new Vector2(0f, 0f);
            endPosition = new Vector2(0f, 0f);

            switch (transitionPreset)
            {
                case TransitionPreset.FadeIn:
                    useAlpha = true;
                    useScale = false;
                    usePosition = false;
                    alphaSpeed = 10f;
                    alphaFadeIn = true;
                    alphaFadeOut = true;
                    break;
                case TransitionPreset.ScaleIn:
                    useAlpha = false;
                    useScale = true;
                    usePosition = false;
                    scaleSpeed = 10f;
                    scaleFadeIn = true;
                    scaleFadeOut = true;
                    break;
                case TransitionPreset.TopDown:
                    useAlpha = true;
                    useScale = false;
                    usePosition = true;
                    alphaSpeed = 10f;
                    moveSpeed = 10f;
                    positionFadeIn = true;
                    positionFadeOut = false;
                    alphaFadeIn = false;
                    alphaFadeOut = true;
                    startPosition = new Vector2(0f, 2000f);
                    localPosition = true;
                    break;
                case TransitionPreset.LeftToRight:
                    useAlpha = false;
                    useScale = false;
                    usePosition = true;
                    moveSpeed = 15f;
                    positionFadeIn = true;
                    positionFadeOut = true;
                    startPosition = new Vector2(-2000f, 0f);
                    endPosition = new Vector2(2000f, 0f);
                    localPosition = true;
                    break;
            }
        }
    }
}
