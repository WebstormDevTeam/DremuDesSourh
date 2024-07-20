using Dremu.Gameplay.Object;
using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Manager;
using Dremu.Gameplay.Tool;
using UnityEngine;


namespace Dremu.Gameplay.Object {

    public sealed class GuideLine : NoteBase {
        [SerializeField] LineRenderer Line;
        [SerializeField] SpriteRenderer Renderer;
        
        [System.Serializable]
        public struct GuideNode {
            public float To, Time;
            public GuideNode( float To, float Time ) {
                this.To = To;
                this.Time = Time;
            }
        }

        List<GuideNode> GuideLineNodes;
        public float NoteEffectTimer;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="CurrentTime"> 当前时间 </param>
        public override void OnActive( float CurrentTime ) {
            float position = this.position;

            //实时更新形状
            var points = new List<Vector2>();
            float start = this.position;
            float time = ArrivalTime;
            Vector2 StartPoint = Vector2.zero;
            for (int i = 0; i < GuideLineNodes.Count; i++) {
                GuideNode Holding = GuideLineNodes[i];

                var pointsPerHolding = JudgmentLine.CurrentCurve.SubCurveByStartAndEnd(start, Holding.To);
                float devide = 1f * (Holding.To - start) / pointsPerHolding.Count;
                float PerDirection = JudgmentLine.Speed.GetPosition(time, Holding.Time) / pointsPerHolding.Count;

                if (points.Count > 0)
                    pointsPerHolding.RemoveAt(0);

                for (int j = 0; j < pointsPerHolding.Count; j++) {
                    KeyValuePair<Vector2, Vector2> normalPerPoint = JudgmentLine.CurrentCurve.GetNormal(start + devide * (j + 1));
                    pointsPerHolding[j] = 
                        StartPoint + 
                        PositionHelper.RelativeCoordToAbsoluteCoord(pointsPerHolding[j], Camera.main) + 
                        (j + 1) * PerDirection * normalPerPoint.Value;
                }

                if (CurrentTime > time && CurrentTime <= time + Holding.Time) {
                    float progress = (CurrentTime - time) / Holding.Time;
                    int index = Mathf.FloorToInt(progress * (pointsPerHolding.Count - 1));
                    float progress_digit = progress * (pointsPerHolding.Count - 1) - index;
                    if (pointsPerHolding.Count - 1 > index)
                        pointsPerHolding[index] += (pointsPerHolding[index + 1] - pointsPerHolding[index]) * progress_digit;
                    position = start + (Holding.To - start) * progress;
                    points.AddRange(pointsPerHolding.GetRange(index, pointsPerHolding.Count - index));
                }
                else if (CurrentTime <= time) {
                    if (points.Count > 0 && points[^1] == pointsPerHolding[0])
                        pointsPerHolding.RemoveAt(0);
                    points.AddRange(pointsPerHolding);
                }

                start = Holding.To;
                time += Holding.Time;
                StartPoint = pointsPerHolding[^1];
            }

            if (points.Count > 0) Line.transform.localPosition = -points[0];
            Line.positionCount = points.Count;
            Line.SetPositions(Functions.Vec2ListToVec3List(points).ToArray());

            //设置音符位置
            KeyValuePair<Vector2, Vector2> normal = JudgmentLine.CurrentCurve.GetNormal(position);
            transform.localPosition = 
                PositionHelper.RelativeCoordToAbsoluteCoord(normal.Key, Camera.main) + 
                (CurrentTime < ArrivalTime ? normal.Value * JudgmentLine.Speed.GetPosition(CurrentTime, ArrivalTime - CurrentTime) : Vector2.zero);

            Renderer.color = Line.startColor = Line.endColor = NoteManager.NoteColor;
        }

        public override void OnInitialize() {
            // NoteEffectTimer = 0;
        }

        public override void OnRecycle() {

        }

        /// <summary>
        /// 设置引导线节点
        /// </summary>
        /// <param name="HoldNodes">节点</param>
        public void SetGuideLineNodes(List<GuideNode> HoldNodes) {
            this.GuideLineNodes = HoldNodes;
        }

        /// <summary>
        /// 是否已经结束
        /// </summary>
        /// <param name="CurrentTime">当前时间</param>
        /// <returns>是否结束</returns>
        public bool IsEnd( float CurrentTime ) {
            float totalTime = ArrivalTime;
            foreach (GuideNode node in GuideLineNodes)
                totalTime += node.Time;
            return CurrentTime >= totalTime;
        }
    }

}
