using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;
using Utils.Helper;
using Utils.Singleton;
using Random = UnityEngine.Random;

namespace GameUI.Global.ColorSet
{
    [DisallowMultipleComponent]
    public class ColorSetManager : MonoBehaviourSingleton<ColorSetManager>
    {
        [Title("时间捏")]
        
        [SerializeField, LabelText("大哥哥❤好❤持❤久❤")]
        private float waitTime;
        
        [SerializeField, LabelText("大哥哥的更换♂时间❤")]
        private float duration;
        
        [Title("列表捏")]
        [SerializeField, LabelText("所有的颜色捏")]
        private Color[] colors;

        [OdinSerialize, LabelText("不会消失的物体捏")] 
        public Graphic[] StaticGraphics { get; private set; }

        [OdinSerialize, LabelText("可能会消失的物体捏"), ReadOnly]
        public List<Graphic> RuntimeGraphics { get; set; } = new(32);
        
        [OdinSerialize, LabelText("边框捏"), ReadOnly]
        public List<UIOutline> RuntimeOutlines { get; set; } = new(32);
        
        public Color NowColor { get; private set; }

        private int _lastIndex = -1;

        private void Start()
        {
            StartCoroutine(ChangeColor());
        }

        private IEnumerator ChangeColor()
        {
            yield return new WaitWhile(() => colors.IsNullOrEmpty());
            
            var colorFirst = GetRandomColor();

            StaticGraphics.ForEach(x => x.color = colorFirst);
            RuntimeGraphics.ForEach(x => x.color = colorFirst);
            RuntimeOutlines.ForEach(x => x.effectColor = colorFirst);

            while (Instance)
            {
                yield return new WaitForSecondsRealtime(waitTime);

                var color = GetRandomColor();
                StaticGraphics.ForEach(x => x.DOColor(color, duration).SetEase(DG.Tweening.Ease.OutSine));
                RuntimeGraphics.ForEach(x => x.DOColor(color, duration).SetEase(DG.Tweening.Ease.OutSine));
                RuntimeOutlines.ForEach(x => x.DoColor(color, duration).SetEase(DG.Tweening.Ease.OutSine));
            }
        }

        private Color GetRandomColor()
        {
            int index;

            do index = Random.Range(0, colors.Length);
            while (index == _lastIndex);

            return NowColor = colors[index].SetAlpha(Random.Range(0.25f, 0.5f));
        }
    }
}
