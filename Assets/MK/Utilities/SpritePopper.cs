using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePopper : MonoBehaviour
{
    [SerializeField] float colorLerpSpeed = 7f;
    [SerializeField] float sizeLerpSpeed = 7f;

    [SerializeField] bool lerpColor = true;
    [SerializeField] bool lerpScale = true;

    [Header("Targets")]
    [SerializeField] bool targetColorIsInit = true;
    [SerializeField] Color targetColor = Color.white;

    [Space]
    [SerializeField] bool targetScaleIsInit = false;
    [SerializeField] Vector3 targetScale = Vector3.zero;


    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (targetColorIsInit)
        {
            targetColor = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
        if (targetScaleIsInit)
        {
            targetScale = transform.localScale;
        }
    }

    private void Update()
    {
        if (lerpColor)
        {
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                targetColor,
                Time.unscaledDeltaTime * colorLerpSpeed
            );
        }

        if (lerpScale)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.unscaledDeltaTime * sizeLerpSpeed
            );
        }
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
