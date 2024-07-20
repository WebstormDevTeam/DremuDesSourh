using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Dremu.Gameplay.Tool;
using Sirenix.OdinInspector;

namespace Dremu.Gameplay.Object {

    /// <summary>
    /// 谱面
    /// </summary>
    [CreateAssetMenu(menuName="谱面/创建谱面文件")]
    public sealed class ChartData : ScriptableObject {

        [Serializable]
        public sealed class JudgmentLineData {

            [Serializable]
            public sealed class Curve {

                [Serializable]
                public sealed class CurveNode {
                    [LabelText("阶数")] public List<Vector2> Nodes;
                }

                [LabelText("端点")] public List<Vector2> Points;
                [LabelText("节点")] public List<CurveNode> Nodes;

            }

            [Serializable]
            public sealed class CurveAction {

                [LabelText("曲线变化包络")] public EnvelopeLineData CurveEnvelope;
                [LabelText("曲线组")] public List<Curve> Curves;

            }

            [Serializable]
            public sealed class NoteData {

                [Serializable]
                public enum NoteType {

                    Tap,
                    Slide,
                    Hold,
                    Drag

                }

                [Serializable]
                public sealed class PerHoldingData {

                    [LabelText("时间")] public float Time;
                    [LabelText("位置"), Range(0, 1)] public float Position;

                }

                [LabelText("音符种类")] public NoteType Type;
                [LabelText("出现位置"), Range(0, 1)] public float Position;
                [LabelText("出现时间")] public float CreateTime;
                [LabelText("抵达时间")] public float ArrivalTime;

                [LabelText("节点")] public List<PerHoldingData> Nodes;

            }

            [LabelText("音符宽度"), Range(0, 1)] public float NoteWidth;

            [LabelText("流速包络")] public EnvelopeLineData SpeedEnvelope;
            [LabelText("角度包络")] public EnvelopeLineData RotateEnvelope;
            [LabelText("X包络")] public EnvelopeLineData MoveXEnvelope;
            [LabelText("Y包络")] public EnvelopeLineData MoveYEnvelope;

            [LabelText("曲线事件")] public CurveAction _CurveAction;

            [LabelText("音符")] public List<NoteData> Notes;

        }

        [Serializable]
        public sealed class EnvelopeLineData {

            [Serializable]
            public sealed class EnvelopeNodeData {

                [LabelText("时间")] public float Time;
                [LabelText("数值")] public float Value;
                [LabelText("类型")] public CurveType type;

            }

            [LabelText("节点信息")] public List<EnvelopeNodeData> Points;

        }

        [LabelText("判定线颜色")] public Color LineColor;
        [LabelText("音符颜色")] public Color NoteColor;

        [LabelText("BPM包络")] public EnvelopeLineData BPMEnvelope;

        [LabelText("判定线")] public List<JudgmentLineData> Lines;

    }
}