using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameUI.Global.ColorSet
{
    [DisallowMultipleComponent]
    public class ColorSetRegister : MonoBehaviour
    {
        [SerializeField, LabelText("要注册的物体捏")] private Graphic[] graphics;
        [SerializeField, LabelText("要注册的边框捏")] private UIOutline[] outlines;

        private bool _canRegister = false;
        
        private void Awake()
        {
            StartCoroutine(CO_WaitIns());
        }

        private IEnumerator CO_WaitIns()
        {
            yield return new WaitWhile(() => ColorSetManager.Instance is null);
            
            ColorSetManager.Instance.RuntimeGraphics ??= new List<Graphic>(32);
            ColorSetManager.Instance.RuntimeGraphics.Clear();
            ColorSetManager.Instance.RuntimeGraphics.AddRange(graphics);
            ChangeColor(graphics);
            
            ColorSetManager.Instance.RuntimeOutlines ??= new List<UIOutline>(32);
            ColorSetManager.Instance.RuntimeOutlines.Clear();
            ColorSetManager.Instance.RuntimeOutlines.AddRange(outlines);
            ChangeColor(outlines);
            
            _canRegister = true;
        }

        public void AddRegister(IList<Graphic> graphic)
        {
            StartCoroutine(CO_AddRegister(graphic));
        }
        
        public void AddRegister(IList<UIOutline> outline)
        {
            StartCoroutine(CO_AddRegister(outline));
        }

        private IEnumerator CO_AddRegister(IList<Graphic> graphic)
        {
            yield return new WaitUntil(() => _canRegister);
            
            ColorSetManager.Instance.RuntimeGraphics.AddRange(graphic);
            ChangeColor(graphic);
        }
        
        private IEnumerator CO_AddRegister(IList<UIOutline> outline)
        {
            yield return new WaitUntil(() => _canRegister);
            
            ColorSetManager.Instance.RuntimeOutlines.AddRange(outline);
            ChangeColor(outline);
        }


        private static void ChangeColor(IList<Graphic> graphics)
        {
            graphics.ForEach(x => x.color = ColorSetManager.Instance.NowColor);
        }
        
        private static void ChangeColor(IList<UIOutline> outlines)
        {
            outlines.ForEach(x => x.effectColor = ColorSetManager.Instance.NowColor);
        }
    }
}
