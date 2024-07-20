using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Tool;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
namespace Dremu.ChartEdit.Envelope
{
    /// <summary>
    /// 一条用来编辑一个包络线的轨道.
    /// 注: 所有的Unchecked最后都要改成private
    /// </summary>
    public class EnvelopeTrack : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler, IPointerMoveHandler, IPointerExitHandler
    {
        public EnvelopeLine Line;
        /// <summary>
        /// 这个List中的物体顺序与子物体顺序相同
        /// </summary>
        [SerializeField] public List<LineContainer> containers;
        [SerializeField] public LineContainer lineContainerPrefab;

        private EditingEntry _editEntry = EditingEntry.None;
        [System.NonSerialized] public UnityAction SelectMe;
        public RectTransform ViewingParent;
        [System.NonSerialized] public VerticalTrackView realParent;
        private bool _isSelected;
        private EnvelopeTrackHelper _helper;
        private Dictionary<EditingEntry, IDragEdit> _editings;
        private static Dictionary<EditingEntry, CursorType> _cursorsForEditing = new Dictionary<EditingEntry, CursorType>
        {
            {EditingEntry.Node, CursorType.Move },
            {EditingEntry.Tension, CursorType.UpDown }
        };

        public float BeatX
        {
            get
            {
                return realParent.BeatX;
            }
        }

        void Start()
        {
            this._editings = new Dictionary<EditingEntry, IDragEdit>
            {
                { EditingEntry.Node, new NodeDragEdit(Line, _helper) },
                { EditingEntry.Tension, new TensionEdit(Line, _helper) },
            };
        }

        #region Setter
        public void SetAllDirty()
        {
            foreach (var container in containers)
            {
                container.SetAllDirty();
            }
        }

        public void SetEnvelopeLine(EnvelopeLine envelopeLine)
        {
            this.Reset();
            this.Line = envelopeLine;
            this._helper = new EnvelopeTrackHelper();
            _helper.SyncTrack(this);
            _helper.PrepareContainer();
        }

        public void SetSelected(bool value)
        {
            // 不同才操作捏
            if (value ^ _isSelected)
            {
                foreach (var container in containers)
                {
                    container.SetSelected(value);
                }
                this._isSelected = value;
            }
        }
        #endregion



        void Reset()
        {
            for (int i = 0; i < containers.Count; i++)
            {
                Destroy(containers[i]);
            }
            this.containers.Clear();
            this.Line = null;
            this._editEntry = EditingEntry.None;
        }

        public void Update()
        {
            // 防止宽高不对劲
            LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
            UpdateChildWidth();
            foreach (LineContainer container in containers)
            {
                container.UpdateByDirty(Line);
            }
        }

        /// <summary>
        /// 在NodeIndex处的node开头切分这个Container.
        /// </summary>
        /// <param name="ContainerIndex"></param>
        /// <param name="NodeIndex">
        /// 切分的地方, 事全局下标; Node不能在它开头或包络线结尾 (就这么定了!(()
        /// 其实在包络线结尾不是不行, 但FitMinMax时会和前面对不上.
        /// </param>
        public void SplitContainerUnchecked(int ContainerIndex, int NodeIndex)
        {
            var goingToSplit = containers[ContainerIndex];
            Debug.Assert(goingToSplit.Contains(NodeIndex));
            Debug.Assert(NodeIndex != goingToSplit.StartIndex && NodeIndex != Line.Controllers.Count - 1);
            // 以前的结尾位置
            var previousEnd = goingToSplit.EndIndex;
            // 从现在的起始到index之前
            goingToSplit.SetNodeRange(goingToSplit.StartIndex, NodeIndex - 1);
            var newContainer = Instantiate(lineContainerPrefab, this.transform);
            // 在列表中正确的位置插入对象 (要分割的对象之后)
            this.containers.Insert(ContainerIndex + 1, newContainer);
            // 设置到正确的显示次序
            newContainer.transform.SetSiblingIndex(ContainerIndex + 1);
            // 从index到以前的结尾
            newContainer.SetNodeRange(NodeIndex, previousEnd);
            // 同步MinMax
            newContainer.SetMinMaxUnchecked(goingToSplit.minValue, goingToSplit.maxValue);
            // 同步选择状态
            newContainer.SetSelected(_isSelected);
        }

        /// <summary>
        /// 将在这个位置的Container与下一个合并.
        /// </summary>
        /// <param name="ContainerIndex"></param>
        public void MergeContainerWithRightUnchecked(int ContainerIndex)
        {
            //最后一个没有下一个捏
            Debug.Assert(ContainerIndex != this.containers.Count - 1);
            // 从这个的开始到下一个的结束
            this.containers[ContainerIndex].SetNodeRange(this.containers[ContainerIndex].StartIndex, containers[ContainerIndex + 1].EndIndex);
            // 移除下一个
            DestroyImmediate(this.containers[ContainerIndex + 1].gameObject);
            this.containers.RemoveAt(ContainerIndex + 1);

        }


        /// <summary>
        /// 添加一个新Node.
        /// </summary>
        /// <param name="ContainerIndex">新Node应该属于的Container</param>
        /// <param name="previousNodeIndex">它上一个Node的下标 (全局) </param>
        /// <param name="time">Node的时间</param>
        /// <param name="value">Node的值</param>
        public void AddNewNode(int ContainerIndex, int previousNodeIndex, float time, float value)
        {
            LineContainer lineContainer = containers[ContainerIndex];
            // 兄弟, 你前面那人已经跑路了, 不在这个Container了
            Debug.Assert(lineContainer.Contains(previousNodeIndex));
            Line.Controllers.Insert(previousNodeIndex + 1, new ControlNode(time, value, 0.5f, CurveType.Expo));
            // 增加一个位置
            lineContainer.SetNodeRange(lineContainer.StartIndex, lineContainer.EndIndex + 1);
            // 我脏了! 呜呜呜~
            lineContainer.SetAllDirty();
            // 我们全都脏了! 都是你害的!
            for (int i = ContainerIndex + 1; i < containers.Count; i++)
            {
                var container = containers[i];
                // 平移
                container.SetNodeRange(container.StartIndex + 1, container.EndIndex + 1);
                container.SetAllDirty();
            }
        }


        private void SetProperEntryAndBeginEdit(Vector2 localPos)
        {
            foreach (var item in _editings)
            {
                if (item.Value.CheckCanEdit(localPos))
                {
                    _editEntry = item.Key;
                    item.Value.StartEdit(localPos);
                    break;
                }
            }
        }



        /// <summary>
        /// 把所有的Container宽度改成适合的值.
        /// </summary>
        private void UpdateChildWidth()
        {

            foreach (LineContainer container in containers)
            {
                // 不是最后一个,那结束时间就是下一个的开始;
                // 是最后的, 结束时间是自己时间+1.
                float endTime;
                if (container.EndIndex == Line.Controllers.Count - 1)
                    endTime = Line.Controllers[^1].Time + 1;
                else
                    endTime = Line.Controllers[container.EndIndex + 1].Time;
                // 时间长度*比率
                var width = (endTime - Line.Controllers[container.StartIndex].Time) * BeatX;
                container.GetComponent<RectTransform>().sizeDelta = new Vector2(width, this.GetComponent<RectTransform>().sizeDelta.y);


            }
        }

        #region Handlers
        /// <summary>
        /// 开始拖动的handler
        /// </summary>
        /// <param name="data"></param>
        public void OnBeginDrag(PointerEventData data)
        {
            SetProperEntryAndBeginEdit(_helper.ScreenPosToLocalPos(data.position));
        }




        public void OnEndDrag(PointerEventData data)
        {
            if (_editEntry != EditingEntry.None)
                _editings[this._editEntry].StopEdit(_helper.ScreenPosToLocalPos(data.position));
        }

        public void OnDrag(PointerEventData data)
        {
            if (_editEntry != EditingEntry.None)
                _editings[this._editEntry].UpdateEdit(_helper.ScreenPosToLocalPos(data.position));
        }


        /// <summary>
        /// 点击时弹出菜单 (以后可能会改)
        /// </summary>
        /// <param name="data"></param>
        public void OnPointerClick(PointerEventData data)
        {
            if (!_isSelected)
                SelectMe();
            var localPos = _helper.ScreenPosToLocalPos(data.position);
            float clickLocalX = localPos.x;
            float time = clickLocalX / BeatX;
            var NodeIndex = EnvelopeLine.GetControlerIndex(Line.Controllers, time);
            var ContainerIndex = _helper.GetContainerIndex(NodeIndex);
            var targetContainer = containers[ContainerIndex];
            float value = (localPos.y) / GetComponent<RectTransform>().sizeDelta.y *
                (targetContainer.maxValue - targetContainer.minValue) + targetContainer.minValue;
            var nearbyNode = _helper.GetNearbyNode(clickLocalX, localPos.y, ContainerIndex);

            if (data.button == PointerEventData.InputButton.Right)
            {
                if (nearbyNode == -1)
                    AddNewNode(ContainerIndex, NodeIndex, time, value);
                else
                {

                    #region ShowMenu
                    var menu = CurveTypeContextHelper.CurveTypeGen(typeof(CurveType), Line.Controllers[nearbyNode], () => { containers[ContainerIndex].SetNodeChanged(nearbyNode, _helper.tryGetPriviousContainer(ContainerIndex)); });
                    menu.AddRange(
                        new List<ContextMenuItem>
                        {

                            new ContextMenuItem(
                                "在此分割", () =>
                                {
                                    SplitContainerUnchecked(ContainerIndex, nearbyNode);
                                }
                            ),
                            new ContextMenuItem(
                                "向右合并", () => MergeContainerWithRightUnchecked(ContainerIndex)
                            ),
                            new ContextMenuItem(
                                "取消", () => {}
                            )
                        }
                    );
                    Debug.Log(data.position);
                    UIManager.Instance.ShowContextMenu(data.position, menu);
                    #endregion
                }
            }
        }

        public void OnPointerMove(PointerEventData data)
        {
            var localPos = _helper.ScreenPosToLocalPos(data.position);
            foreach (var item in this._editings)
            {
                if (item.Value.CheckCanEdit(localPos))
                {
                    UIManager.Instance.SetCursorType(_cursorsForEditing[item.Key]);
                    break;
                }
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            UIManager.Instance.SetCursorType(CursorType.Normal);
        }
        #endregion Handlers


    }

    public enum EditingEntry
    {
        Tension = 0,
        Node,
        None
    }
}
