using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Tool;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dremu.ChartEdit.Envelope
{
    public interface IDragEdit
    {
        public void SetLine(EnvelopeLine line);
        public void SetHelper(EnvelopeTrackHelper helper);
        public void StartEdit(Vector2 localPos);
        public void StopEdit(Vector2 localPos);
        public void UpdateEdit(Vector2 localPos);
        public bool CheckCanEdit(Vector2 localPos);
    }

    public abstract class EditingBase<T> : IDragEdit
    {
        protected T _storedLastValue;
        protected Vector2 _startDragPos;
        protected int _editIndex;
        protected int _containerIndex;

        protected bool _inEditing;

        protected EnvelopeLine _envelopeLine;

        protected EnvelopeTrackHelper _helper;

        public EditingBase(EnvelopeLine envelope, EnvelopeTrackHelper helper) {
            this._envelopeLine = envelope;
            this._helper = helper;
        }

        public void SetLine(EnvelopeLine envelopeLine)
        {
            this._envelopeLine = envelopeLine;
        }

        public void SetHelper(EnvelopeTrackHelper helper)
        {
            this._helper = helper;
        }

        public abstract void StartEdit(Vector2 localPos);
        public abstract void StopEdit(Vector2 localPos);
        public abstract void UpdateEdit(Vector2 localPos);
        public abstract bool CheckCanEdit(Vector2 localPos);

    }

    public class NodeDragEdit: EditingBase<Vector2>
    {
        public NodeDragEdit(EnvelopeLine envelope, EnvelopeTrackHelper helper) : base(envelope, helper)
        {
        }

        public override void StartEdit(Vector2 localPos)
        {
            Debug.Assert(!_inEditing, "Try start when started");
            _inEditing = true;
            // x 加一点, 在末尾的时候就被判定为下一个container
            var targetIndex = _helper.GetNodeIndexForX(localPos.x);
            _containerIndex = _helper.GetContainerIndex(targetIndex);
            _editIndex = _helper.GetNearbyNode(localPos.x, localPos.y, _containerIndex);
            _storedLastValue = new Vector2(_envelopeLine.Controllers[_editIndex].Value, _envelopeLine.Controllers[_editIndex].Time);
            _startDragPos = localPos;
        }

        public override void UpdateEdit(Vector2 currentPos)
        {
            Debug.Assert(_inEditing, "Try update when not in editing");
            ControllNode editNode = _envelopeLine.Controllers[_editIndex];
            var maxAllowedTime = _editIndex != _envelopeLine.Controllers.Count - 1 ? _envelopeLine.Controllers[_editIndex + 1].Time : float.MaxValue;
            var minAllowedTime = 0f;
            if (_editIndex == 0)
            {
                maxAllowedTime = 0f;
            }
            else
            {
                minAllowedTime = _envelopeLine.Controllers[_editIndex - 1].Time;
            }

            var xDelta = currentPos.x - _startDragPos.x;
            var yDelta = currentPos.y - _startDragPos.y;
            var container = _helper.GetContainer(_containerIndex);
            var ValueDelta = (yDelta / _helper.TrackRectTransform.sizeDelta.y) * (container.maxValue - container.minValue);
            var newValue = Mathf.Clamp(_storedLastValue.x + ValueDelta, container.minValue, container.maxValue);
            var newTime = (_storedLastValue.y + xDelta / _helper.BeatX);
            newTime = Mathf.Clamp(newTime, minAllowedTime, maxAllowedTime);
            editNode.Time = SnipHelper.Snip(newTime);
            editNode.Value = newValue;
            // 要更新的Node: editIndex
            container.SetNodeChanged(_editIndex, _helper.tryGetPriviousContainer(_containerIndex), SnipHelper.Snip(newTime));
        }

        public override void StopEdit(Vector2 localPos)
        {
            Debug.Assert(_inEditing, "Try stop when stopped");
            _inEditing = false;
        }

        public override bool CheckCanEdit(Vector2 localPos)
        {
            // x 加一点, 在末尾的时候就被判定为下一个container
            var targetIndex = _helper.GetNodeIndexForX(localPos.x + 20);
            var containerIndex = _helper.GetContainerIndex(targetIndex);
            return _helper.GetNearbyNode(localPos.x, localPos.y, containerIndex) != -1;
        }
    }
    public class TensionEdit : EditingBase<float>
    {
        const float Y_PER_TENSION = 100;

        public TensionEdit(EnvelopeLine envelope, EnvelopeTrackHelper helper) : base(envelope, helper)
        {
        }

        public override void StartEdit(Vector2 localPos)
        {
            Debug.Assert(!_inEditing, "Try start when started");
            _inEditing = true;
            _editIndex = _helper.GetNodeIndexForX(localPos.x);
            Debug.Assert(_editIndex != -1, "Are you Serious??");
            _containerIndex = _helper.GetContainerIndex(_editIndex);
            _storedLastValue = _envelopeLine.Controllers[_editIndex].Tension;
            _startDragPos = localPos;
        }

        public override void UpdateEdit(Vector2 localPos)
        {
            Debug.Assert(_inEditing, "Try update when not in editing");

            // drag后移动的y位移
            var yDelta = localPos.y - _startDragPos.y;
            // 新的Tension是原来的加delta (并没有绝对值)
            var newTension = Mathf.Clamp01(_storedLastValue + yDelta / Y_PER_TENSION);

            // 最后一个没有下一个捏
            if (_editIndex != _envelopeLine.Controllers.Count - 1)
            {
                // 如果它是倒着走的, 那函数的突起方向实际上和delta方向相反, 要把tension倒过来
                if (_envelopeLine.Controllers[_editIndex].Value > _envelopeLine.Controllers[_editIndex + 1].Value)
                {
                    newTension *= -1;
                    newTension += 1;
                }
            }


            _envelopeLine.Controllers[_editIndex].Tension = newTension;
            // 要更新的Node: editIndex
            _helper.GetContainer(_containerIndex).SetNodeChanged(_editIndex, _helper.tryGetPriviousContainer(_containerIndex));

        }

        public override void StopEdit(Vector2 localPos)
        {
            Debug.Assert(_inEditing, "Try stop when stopped");
            _inEditing = false;
        }

        public override bool CheckCanEdit(Vector2 localPos)
        {
            //其实总是能
            return true;
        }
    }
}