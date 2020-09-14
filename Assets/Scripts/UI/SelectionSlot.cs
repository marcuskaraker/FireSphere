using MK.UI;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSlot : MonoBehaviour
{
    public Image frameImage;
    public Image backgroundImage;
    public Image iconImage;

    public HealthBar durabilityBar;

    public bool IsSelected { get; private set; }

    public void Select(bool value)
    {
        frameImage.enabled = value;
        IsSelected = value;
    }

    public void SetIcon(Sprite sprite)
    {
        SetIcon(sprite, Color.white);
    }

    public void SetIcon(Sprite sprite, Color color)
    {
        iconImage.sprite = sprite;
        iconImage.color = color;
        iconImage.enabled = (sprite != null);
    }
}
