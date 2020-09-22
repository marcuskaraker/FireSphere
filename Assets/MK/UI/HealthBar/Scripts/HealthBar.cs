using UnityEngine;
using UnityEngine.UI;

namespace MK.UI
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        public Image fillImage;

        public Slider Slider { get; private set; }
        public Text Text { get; private set; }

        private Color defaultFillColor;

        public Color FillColor
        {
            get
            {
                return fillImage.color;
            }
            set
            {
                fillImage.color = value;
            }
        }

        private void Awake()
        {
            Slider = GetComponent<Slider>();
            Text = GetComponentInChildren<Text>();

            defaultFillColor = fillImage.color;
        }

        public void SetValue(float value)
        {
            if (Slider == null) return;

            Slider.value = value;
        }

        public void SetValue(float value, string text)
        {
            if (Slider == null) return;

            SetValue(value);

            if (Text)
            {
                Text.text = text;
            }
        }

        public void ResetFillColor()
        {
            fillImage.color = defaultFillColor;
        }
    }
}
