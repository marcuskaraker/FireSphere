using UnityEngine;
using UnityEngine.UI;

namespace MK.UI
{
    [RequireComponent(typeof(Slider))]
    public class HealthBar : MonoBehaviour
    {
        public Slider Slider { get; private set; }
        public Text Text { get; private set; }

        private void Awake()
        {
            Slider = GetComponent<Slider>();
            Text = GetComponentInChildren<Text>();
        }

        public void SetValue(float value)
        {
            Slider.value = value;
        }

        public void SetValue(float value, string text)
        {
            SetValue(value);

            if (Text)
            {
                Text.text = text;
            }
        }
    }
}
