using UnityEngine;
using UnityEngine.UI;

public class LerpColorer : MonoBehaviour
{
    public SpriteRenderer targetSprite;
    public Image targetImage;
    public Text text;

    [Space]

    public Color targetColor = Color.white;
    public Color setColor = Color.red;
    public float lerpSpeed = 7;

    private void Update()
    {
        if (targetSprite != null)
        {
            targetSprite.color = Color.Lerp(targetSprite.color, targetColor, lerpSpeed * Time.deltaTime);
        }

        if (targetImage != null)
        {
            targetImage.color = Color.Lerp(targetImage.color, targetColor, lerpSpeed * Time.deltaTime);
        }

        if (text != null)
        {
            text.color = Color.Lerp(text.color, targetColor, lerpSpeed * Time.deltaTime);
        }
    }

    public void SetColor()
    {
        if (targetSprite != null)
        {
            targetSprite.color = setColor;
        }

        if (targetImage != null)
        {
            targetImage.color = setColor;
        }

        if (text != null)
        {
            text.color = setColor;
        }
    }
}
