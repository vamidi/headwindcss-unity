using UnityEngine;

namespace HeadWindCSS.Domains.Extensions.UI.Elements
{
    public static class ColorHelper
    {
        public static Color ParseHtmlColor(string color)
        {
            if (ColorUtility.TryParseHtmlString(color, out var result))
            {
                return result;
            }

            return Color.clear;
        }
        
        public static string ToHtmlString(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }
        
        public static string ToRGBHex(this Color color)
        {
            return $"#{ToByte(color.r):X2}{ToByte(color.g):X2}{ToByte(color.b):X2}";
        }
        
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }
}