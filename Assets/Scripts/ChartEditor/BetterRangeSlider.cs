using System.Collections;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;

namespace Dremu.ChartEdit
{
    /// <summary>
    /// 原先的slider总把中心移到鼠标位置
    /// </summary>
    public class BetterRangeSlider : RangeSlider, IBeginDragHandler
    {
        private float _lastNormalizedLow;
        private float _lastNormalizedHigh;
        public virtual void OnBeginDrag(PointerEventData data)
        {
            _lastNormalizedLow = NormalizedLowValue;
            _lastNormalizedHigh = NormalizedHighValue;
        }


        protected override void CalculateBarDrag(PointerEventData eventData, Camera cam)
        {
            #region Copyed
            RectTransform clickRect = m_FillContainerRect;
            if (clickRect != null && clickRect.rect.size[0] > 0)
            {
                Vector2 localCursor;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.position, cam, out localCursor))
                {
                    return;
                }
                localCursor -= clickRect.rect.position;

                #endregion

                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, eventData.pressPosition, cam, out Vector2 beginCursor))
                {
                    return;
                }
                beginCursor -= clickRect.rect.position;
                #region Copyed
                //now we need to get the delta hold on the bar
                //and move both the normalized low and high values by this amount
                //but also check that neither is going beyond the bounds
                if (NormalizedLowValue >= 0 && NormalizedHighValue <= 1)
                {
                    #endregion
                    // 原先是找到中点, 现在我们需要的是点击点
                    float clickedNormalPos = Mathf.Clamp01((beginCursor)[0] / clickRect.rect.size[0]);
                    float clickedPercent = Mathf.InverseLerp(_lastNormalizedLow, _lastNormalizedHigh,clickedNormalPos);
                    float clicked = Mathf.Lerp(NormalizedLowValue, NormalizedHighValue, clickedPercent);
                    Debug.Log(clickedPercent);
                    #region Copyed
                    //find where the new mid point should be
                    float val = Mathf.Clamp01((localCursor)[0] / clickRect.rect.size[0]);
                    //calculate the delta
                    float delta = val - clicked;
                    //check the clamp range
                    if (NormalizedLowValue + delta < 0)
                    {
                        delta = -NormalizedLowValue;
                    }
                    else if (NormalizedHighValue + delta > 1)
                    {
                        delta = 1 - NormalizedHighValue;
                    }

                    //adjust both ends
                    NormalizedLowValue += delta;
                    NormalizedHighValue += delta;
                }
            }
            #endregion
        }
    }
}