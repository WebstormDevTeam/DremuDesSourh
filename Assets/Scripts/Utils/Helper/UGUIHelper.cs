using UnityEngine;

namespace Utils.Helper
{
    public static class UGUIHelper
    {
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new (color.r, color.g, color.b, Mathf.Clamp01(alpha));
        }
        
        public static Color32 SetAlpha(this Color32 color, int alpha)
        {
            return new (color.r, color.g, color.b, (byte) Mathf.Clamp(alpha, 0, 255));
        }
    }
}
