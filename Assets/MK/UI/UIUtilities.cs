using UnityEngine;

namespace MK.UI
{
    public static class UIUtilities
    {
        public static string ColorText(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
        }

        public static string StyleText(string text, FontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case FontStyle.Bold:
                    return "<b>" + text + "</b>";
                case FontStyle.Italic:
                    return "<i>" + text + "</i>";
                case FontStyle.BoldAndItalic:
                    return "<b><i>" + text + "</i></b>";
            }

            return text;
        }

        public static string StyleAndColorText(string text, FontStyle fontStyle, Color color)
        {
            text = StyleText(text, fontStyle);
            return ColorText(text, color);
        }
    }
}

