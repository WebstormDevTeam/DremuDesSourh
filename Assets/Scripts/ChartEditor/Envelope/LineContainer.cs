using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Dremu.Gameplay.Tool;

namespace Dremu.ChartEdit.Envelope
{

    /// <summary>
    /// 主要是一个物体不能有多个graphics 组件, 所以才把UILine放在子物体里.
    /// </summary>
    public class LineContainer : MonoBehaviour
    {
        const int SECTION_BETWEEN_NODES = 50;
        public float minValue { get; private set; }
        public float maxValue { get; private set; }
        public float startTime { get; private set; }
        [SerializeField] RectTransform containsUILine;
        [SerializeField] InputField maxField;
        [SerializeField] InputField minField;
        private List<bool> _dirtyNodes = new List<bool>();
        private bool _allDirty = true;
        private bool _lastPointDirty = false;
        [System.NonSerialized] public EnvelopeTrack Parent;
        private Vector2 _viewParentPos
        {
            get
            {
                return Parent.ViewingParent.position;
            }
        }
        public int StartIndex {
            get; private set;
        }
        public int EndIndex {
            get; private set;
        }

        private float _height;
        private float _width;

        public void SetSelected(bool value)
        {
            this.maxField.gameObject.SetActive(value);
            this.minField.gameObject.SetActive(value);
        }

        private void UpdateInputFieldX()
        {
            var screenLeftBoundLocalX = ScreenPosToLocalPos(_viewParentPos).x;
            var newInputFieldX = Mathf.Clamp(screenLeftBoundLocalX, 0, _width - this.maxField.GetComponent<RectTransform>().sizeDelta.x);
            maxField.transform.localPosition = new Vector2(newInputFieldX, maxField.transform.localPosition.y);
            minField.transform.localPosition = new Vector2(newInputFieldX, minField.transform.localPosition.y);
        }

        /// <summary>
        /// 转换屏幕坐标到局部坐标
        /// </summary>
        /// <param name="ScreenPos">屏幕坐标</param>
        /// <returns>转换的局部坐标</returns>
        private Vector2 ScreenPosToLocalPos(Vector2 ScreenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), ScreenPos, null, out ScreenPos);

            return ScreenPos;
        }


        public void SetAllDirty()
        {
            this._allDirty = true;
            this._lastPointDirty = true;
        }

#nullable enable
        public void SetNodeChanged(int globalIndex, LineContainer? previousCotainer, float? newStartTime = null)
        {
            this._dirtyNodes[globalIndex - StartIndex] = true;
            // 某个node变动会使它之前的node发生变化.
            if (globalIndex - StartIndex - 1 >= 0)
                this._dirtyNodes[globalIndex - StartIndex - 1] = true;
            else if (previousCotainer != null)
            {
                this.startTime = newStartTime ?? this.startTime;
                previousCotainer._lastPointDirty = true;
            }
        }
#nullable disable


        /// <summary>
        /// 设置从哪个Node到哪个Node.
        /// StartIndex和EndIndex都含在内
        /// </summary>
        /// <param name="StartIndex">node 起始</param>
        /// <param name="EndIndex">node 结束</param>
        public void SetNodeRange(int StartIndex, int EndIndex)
        {
            this.StartIndex = StartIndex;
            this.EndIndex = EndIndex;
            // UILine需要的点数量
            int TotalPointCount = (EndIndex - StartIndex + 1) * SECTION_BETWEEN_NODES + 1;
            var points = this.containsUILine.GetComponent<UILine>().points;
            var needToAddOrRemove = TotalPointCount - points.Count; 
            var needToAddOrRemoveNode = (EndIndex - StartIndex + 1) - _dirtyNodes.Count;
            if (needToAddOrRemove < 0)
            {
                points.RemoveRange(points.Count + needToAddOrRemove, -needToAddOrRemove);
                _dirtyNodes.RemoveRange(_dirtyNodes.Count + needToAddOrRemoveNode, -needToAddOrRemoveNode);
            }
            else
            {
                float lastPointX;
                try
                {
                    lastPointX = points[^1].x;
                }
                catch
                {
                    lastPointX = 0;
                }

                for (int i = 1; i < needToAddOrRemove + 1; i++)
                {
                    Debug.Assert(Parent != null);
                    points.Add(new Vector2(lastPointX + i * (Parent.BeatX / SECTION_BETWEEN_NODES), 0f));
                }
                for (int i = 0; i < needToAddOrRemoveNode; i++)
                {
                    _dirtyNodes.Add(true);
                }
            }
            _lastPointDirty = true;
        }


        private void Start()
        {
            this.maxField.onSubmit.AddListener((value) => { this.SetMinMaxUnchecked(this.minValue, float.Parse(value)); });
            this.minField.onSubmit.AddListener((value) => { this.SetMinMaxUnchecked(float.Parse(value), this.maxValue); });
            UnityAction<string> SyncValue = (string _value) => { this.minField.text = this.minValue.ToString(); this.maxField.text = this.maxValue.ToString(); };
            this.maxField.onEndEdit.AddListener(SyncValue);
            this.minField.onEndEdit.AddListener(SyncValue);
            SyncValue("");
        }

        /// <summary>
        /// 设置Container的最大值和最小值.
        /// </summary>
        /// <param name="MinValue"></param>
        /// <param name="MaxValue"></param>
        /// public是debug的!如果最后做完了没改谁看见就改了.
        public void SetMinMaxUnchecked(float MinValue, float MaxValue)
        {
            this.minValue = MinValue;
            this.maxValue = MaxValue;
            this.SetAllDirty();
        }

        void Update()
        {
            UpdateInputFieldX();
            _height = this.GetComponent<RectTransform>().rect.height;
            _width = this.GetComponent<RectTransform>().rect.width;
            containsUILine.sizeDelta = new Vector2(_width, _height);
        }

        private void SyncOneNode(ControlNode node, int globalIndex, float endValue, float duration = 1f)
        {
            Debug.Assert(this.Contains(globalIndex), "找不到那么多 Node 捏 (" + globalIndex + ")");
            var index = globalIndex - this.StartIndex;
            UILine uiLine = containsUILine.GetComponent<UILine>();
            var targetNodePoint = index * SECTION_BETWEEN_NODES;
            var targetY = (node.Value - minValue) / (maxValue - minValue) * _height;
            var endY = (endValue - minValue) / (maxValue - minValue)*_height;
            uiLine.points[targetNodePoint] = new Vector2(Parent.BeatX*(node.Time-this.startTime), targetY); 
            for (int i = 1; i < SECTION_BETWEEN_NODES; i++)
            {
                // n段就有n-1个点 (起始和结束不算)
                var percent = (float) i /  (float) (SECTION_BETWEEN_NODES-1);
                var result = node.ValueGetPercent(percent);
                uiLine.points[targetNodePoint + i] = new Vector2(uiLine.points[targetNodePoint].x+ i* ((Parent.BeatX * duration) / SECTION_BETWEEN_NODES), (endY - targetY) * result + targetY);
            }

            // 下一个node的起始点是下一个node的事
            // 还有一种可能,已经到了单个Container的结尾, 这在SyncNextStartValue里更新
        }

        private void SyncNextStartValue(EnvelopeLine envelopeLine)
        {
            UILine uiLine = containsUILine.GetComponent<UILine>();
            if (this.EndIndex < envelopeLine.Controllers.Count - 1)
            {
                uiLine.points[^1] = new Vector2(uiLine.points[^1].x, _height * (envelopeLine.Controllers[EndIndex + 1].Value - minValue) / (maxValue - minValue));
            }
            else
            {
                uiLine.points[^1] = new Vector2(uiLine.points[^1].x, _height * (envelopeLine.Controllers[EndIndex].Value - minValue) / (maxValue - minValue));
            }
        }
        private void SyncNextStartTime(EnvelopeLine envelope)
        {
            UILine uiLine = containsUILine.GetComponent<UILine>();

            if (this.EndIndex < envelope.Controllers.Count - 1)
            {
                uiLine.points[^1] = new Vector2((envelope.Controllers[EndIndex+1].Time - this.startTime) *Parent.BeatX, uiLine.points[^1].y);
            }
            else
            {
                uiLine.points[^1] = new Vector2((envelope.Controllers[^1].Time + 1 - this.startTime)*Parent.BeatX,uiLine.points[^1].y);
            }
        }

        private void SyncAllNodes(EnvelopeLine envelopeLine)
        {
            for (var i = StartIndex; i <= EndIndex; i++)
            {
               
                if (i == envelopeLine.Controllers.Count - 1)
                    SyncOneNode(envelopeLine.Controllers[i], i, envelopeLine.Controllers[i].Value);
                else
                    SyncOneNode(envelopeLine.Controllers[i], i, envelopeLine.Controllers[i+1].Value,
                        envelopeLine.Controllers[i + 1].Time - envelopeLine.Controllers[i].Time);
            }
            containsUILine.GetComponent<UILine>().points[^1] = new Vector2(
                containsUILine.GetComponent<UILine>().points[^1].x, 
                (envelopeLine.Controllers[^1].Value - minValue) / (maxValue - minValue) * _height
                );
        }

        private void SyncEnvelopeFull(EnvelopeLine envelopeLine)
        {
            this.startTime = envelopeLine.Controllers[StartIndex].Time;
            SyncAllNodes(envelopeLine);
            this._allDirty = false;
            this._dirtyNodes = new List<bool>(Enumerable.Repeat(false, _dirtyNodes.Count));
        }

        public void FitMinMax(EnvelopeLine envelopeLine)
        {
            var minValue = envelopeLine.Controllers[StartIndex].Value;
            var maxValue = minValue;
            for (var i = StartIndex + 1; i <= EndIndex + 1; i++)
            {
                if (i == envelopeLine.Controllers.Count)
                    break;
                
                minValue = Mathf.Min(minValue, envelopeLine.Controllers[i].Value);
                maxValue = Mathf.Max(maxValue, envelopeLine.Controllers[i].Value);
            }
            if (minValue == maxValue)
            {
                // 一样会导致Invalid AABB
                minValue -= 0.5f;
                maxValue += 0.5f;
            }
            SetMinMaxUnchecked(minValue, maxValue);
        }

        public void UpdateByDirty(EnvelopeLine envelopeLine)
        {
            if (_height == 0)
                //没加载好
                return;
            if (_lastPointDirty)
            {
                SyncNextStartValue(envelopeLine);
                SyncNextStartTime(envelopeLine);
                this._dirtyNodes[^1] = true;
            }

            if (!this._allDirty)
            {
                for (int i = this.StartIndex; i <= this.EndIndex; i++)
                {
                    if (_dirtyNodes[i - this.StartIndex])
                    {
                        if (i >= envelopeLine.Controllers.Count - 1)
                            SyncOneNode(envelopeLine.Controllers[i], i, envelopeLine.Controllers[i].Value);
                        else
                            SyncOneNode(envelopeLine.Controllers[i], i, envelopeLine.Controllers[i + 1].Value, 
                                envelopeLine.Controllers[i+1].Time - envelopeLine.Controllers[i].Time);
                        _dirtyNodes[i - this.StartIndex] = false;
                    }
                }
            }
            else
                SyncEnvelopeFull(envelopeLine);
            

        }

        public int GetIndexForX(float localX, EnvelopeLine envelope)
        {
            float time = localX /Parent.BeatX + envelope.Controllers[StartIndex].Time;
            var ctrl = EnvelopeLine.GetControlerIndex(envelope.Controllers, time, StartIndex, EndIndex);
            return ctrl;
        }

        public bool Contains(int globalIndex)
        {
            return globalIndex <= this.EndIndex && globalIndex >= this.StartIndex;
        }

        /// <summary>
        /// 此 Container 是否包含某个位置.
        /// 它不能是最后一个或第一个.
        /// </summary>
        /// <param name="globalIndex"></param>
        /// <returns></returns>
        public bool ContainsStrict(int globalIndex)
        {
            return globalIndex < this.EndIndex && globalIndex > this.StartIndex;
        }
    }
} 

