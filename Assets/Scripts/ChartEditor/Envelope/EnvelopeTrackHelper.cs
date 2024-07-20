using System.Collections.Generic;
using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.ChartEdit.Envelope
{
    public class EnvelopeTrackHelper
    {
        private List<LineContainer> _containers;
        private EnvelopeLine _line;
        public RectTransform TrackRectTransform
        {
            get; private set;
        }
        private EnvelopeTrack _track;

        private LineContainer _lineContainerPrefab;

        public void SyncTrack(EnvelopeTrack track)
        {
            this._containers = track.containers;
            this._line = track.Line;
            this.TrackRectTransform = track.GetComponent<RectTransform>();
            this._lineContainerPrefab = track.lineContainerPrefab;
            this._track = track;
        }

        public LineContainer GetContainer(int index)
        {
            return this._containers[index];
        }

        public float
        BeatX
        {
            get
            {
                return _track.BeatX;
            }
        }

        #region Helper
        /// <summary>
        /// 为包络线准备好初始的Container
        /// </summary>
        public void PrepareContainer()
        {
            var container = Object.Instantiate(_lineContainerPrefab, TrackRectTransform.transform);
            container.gameObject.SetActive(true);
            container.Parent = _track;
            // 包含整个envelope
            container.SetNodeRange(0, _line.Controllers.Count - 1);
            // 全需要更新
            container.SetAllDirty();
            // 自适应大小
            container.FitMinMax(_line);


            this._containers.Add(container);
        }
        /// <summary>
        /// 顺序搜索, 大概不会有那么多...吧?
        /// </summary>
        public int GetContainerIndex(int NodeIndex)
        {
            for (int i = 0; i < this._containers.Count; i++)
            {
                if (_containers[i].Contains(NodeIndex))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 顺序搜索找到起始点在附近的最后一个Node
        /// </summary>
        /// <param name="localX">局部X坐标 (EnvelopeTrack) </param>
        /// <param name="xRange">相差距离在多少以内算在附近</param>
        /// <returns></returns>
        public int GetNearbyNode(float localX, float localY, int targetContainerIndex, float xRange = 20f, float yRange = 20f)
        {
            float time = localX / _track.BeatX;
            float coefficient = (_containers[targetContainerIndex].maxValue - _containers[targetContainerIndex].minValue) / TrackRectTransform.sizeDelta.y;
            float value = localY * coefficient;
            for (int i = _line.Controllers.Count - 1; i >= 0; i--)
            {
                if (Mathf.Abs(time - _line.Controllers[i].Time) * _track.BeatX < xRange &&
                    (Mathf.Abs(value - _line.Controllers[i].Value + _containers[targetContainerIndex].minValue) / coefficient) < yRange)
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// 试图拿到前一个Container
        /// </summary>
        /// <param name="index">要拿的位置</param>
        /// <returns>index不为0时返回前一个, 否则返回null</returns>
        public LineContainer tryGetPriviousContainer(int index)
        {
            // 0没有前一个捏
            if (index == 0)
                return null;
            else
                return _containers[index - 1];
        }

        public int GetNodeIndexForX(float x)
        {
            return EnvelopeLine.GetControlerIndex(_line.Controllers, x / _track.BeatX);
        }


        /// <summary>
        /// 转换屏幕坐标到局部坐标
        /// </summary>
        /// <param name="ScreenPos">屏幕坐标</param>
        /// <returns>转换的局部坐标</returns>
        public Vector2 ScreenPosToLocalPos(Vector2 ScreenPos, RectTransform transform = null)
        {
            if (transform == null)
                transform = TrackRectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, ScreenPos, null, out ScreenPos);
            return ScreenPos;
        }
        #endregion Helper
    }
}