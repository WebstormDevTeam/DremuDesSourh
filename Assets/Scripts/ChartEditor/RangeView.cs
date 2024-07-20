using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Dremu.ChartEdit
{

    /// <summary>
    /// 其实还是偶尔会有点难受的bug, 不过就这样吧
    /// </summary>
    public class RangeView : MonoBehaviour
    {
        [SerializeField] ScrollRect scrollRect;
        RectTransform _scrollRectTransform;
        [SerializeField] RangeSlider rangeSlider;

        public UnityEvent<float> onWidthChanged;
        [Range(0, 1)] float _minLength;
        public float MinLength
        {
            get
            {
                return _minLength;
            }
            set
            {
                _minLength = value;
            }
        }
        private bool _doNotUpdateRangeForOnce;

        private void Start()
        {
            this._scrollRectTransform = scrollRect.GetComponent<RectTransform>();
            rangeSlider.OnValueChanged.AddListener(RangeUpdateHandler);
            scrollRect.onValueChanged.AddListener(ScrollRectHandler);
        }

        private void ScrollRectHandler(Vector2 value)
        {

            if (_doNotUpdateRangeForOnce)
            {
                _doNotUpdateRangeForOnce = false;
                return;
            }
            // 已经到最右边就不更新了, 防止滚动条瞬移
            if (Mathf.Approximately(value.x, 1f))
            {
                return;
            }
            var length = rangeSlider.HighValue - rangeSlider.LowValue;
            rangeSlider.SetValueWithoutNotify(Mathf.Lerp(0, 1 - length, value.x), Mathf.Lerp(length, 1, value.x));
            
        }

        private void RangeUpdateHandler(float left, float right)
        {
            (left, right) = ClampLeftRight(left, right, 0.2f);
            var newWidth = ComputeNewWidth(right - left);
            var leftX = -ComputeLeftX(left, newWidth);
            scrollRect.content.sizeDelta = new Vector2(newWidth, scrollRect.content.sizeDelta.y);
            onWidthChanged.Invoke(newWidth);
            scrollRect.content.localPosition = new Vector2(leftX, scrollRect.content.localPosition.y);
            rangeSlider.SetValueWithoutNotify(left, right);
            _doNotUpdateRangeForOnce = true;
        }

        /// <summary>
        /// 计算出新的宽度
        /// </summary>
        /// <param name="RangeLength">范围长度的百分比</param>
        /// <returns>计算的宽度</returns>
        private float ComputeNewWidth(float RangeLength)
        {
            var factor = 1 / RangeLength;
            var width = _scrollRectTransform.rect.width;
            width *= factor;
            return width;
        }

        /// <summary>
        /// 计算左边缘的局部横坐标
        /// </summary>
        /// <param name="leftPercent"></param>
        /// <returns></returns>
        private float ComputeLeftX(float leftPercent, float newWidth)
        {
            return Mathf.Lerp(0, newWidth, leftPercent);
        }

        /// <summary>
        /// 确保总长度不会小于一个值
        /// </summary>
        /// <param name="leftPercent">左边的百分比</param>
        /// <param name="rightPercent">右边的百分比</param>
        /// <param name="minLength"></param>
        /// <returns>计算得的左右值</returns>
        private Tuple<float, float> ClampLeftRight(float leftPercent, float rightPercent, float minLength)
        {
            // 优先确保左边缘
            if (rightPercent > minLength)
            {
                leftPercent = Mathf.Clamp(leftPercent, 0, rightPercent - minLength);
            }
            else
            {
                rightPercent = minLength;
                leftPercent = 0;
            }
            return Tuple.Create(leftPercent, rightPercent);
        }
    }
}
