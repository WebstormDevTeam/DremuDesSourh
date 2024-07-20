using DG.Tweening;
using GameUI.Global;
using UnityEngine;
using UnityEngine.UI;

namespace Utils.Helper
{
    public static class DoTweenHelper
    {
        public static Tween DoColor(this UIOutline outline, Color color, float duration)
        {
            return DOTween.To(() => outline.effectColor, x => outline.effectColor = x, color, duration);
        }
        
        public static Tween DoColor(this Graphic graphic, Color color, float duration)
        {
            return DOTween.To(() => graphic.color, x => graphic.color = x, color, duration);
        }
        
        public static Tween DoFade(this Graphic graphic, float alpha, float duration)
        {
            var color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, alpha);
            return graphic.DoColor(color, duration);
        }

        public static Tween DoFade(this CanvasGroup group, float alpha, float duration)
        {
            return DOTween.To(() => group.alpha, x => group.alpha = x, alpha, duration);
        }

        public static Tween DoVolume(this AudioSource source, float volume, float duration)
        {
            return DOTween.To(() => source.volume, x => source.volume = x, volume, duration);
        }

        public static Tween DoFill(this Image image, float fill, float duration)
        {
            return DOTween.To(() => image.fillAmount, x => image.fillAmount = x, fill, duration);
        }
        
        public static Tween DoSize(this Image image, Vector2 size, float duration)
        {
            return DOTween.To(() => image.rectTransform.sizeDelta, x => image.rectTransform.sizeDelta = x, size, duration);
        }
    }
}
