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
        //引导线上的节点
        public struct GuideNode {
            //To:终点位置（相对于判定线）
            //Time:持续时间
            public float To, Time;
            public EaseTypeManager.EaseType EaseType;
            public GuideNode( float To, float Time, EaseTypeManager.EaseType EaseType) {
                this.To = To;
                this.Time = Time;
                this.EaseType = EaseType;
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
            
            //points：所有点的集合
            var points = new List<Vector2>();
            //初始化起点，以及到达判定线的时间
            float start = this.position;
            float time = ArrivalTime;
            Vector2 StartPoint = Vector2.zero;

            //对于当前引导线：
            for (int i = 0; i < GuideLineNodes.Count; i++) {
                GuideNode Holding = GuideLineNodes[i];
                //利用新函数使选取的曲线呈现缓动函数形态
                var pointsPerHolding = JudgementLine.CurrentCurve.SubCurveByStartAndEnd(start, Holding.To, Holding.EaseType);
                
                //计算相对于起点，每个点下落空间(横轴)的位置
                List<float> divide = EaseTypeManager.GetEaseLine(Holding.To - start, pointsPerHolding.Count, Holding.EaseType);
                //相对于起点，每个点下落时间(纵轴)的位置变化量，注意每个点的时间间隔是相同的所以此处不用修改保持线性
                float perDirection = JudgementLine.Speed.GetPosition(time, Holding.Time) / pointsPerHolding.Count;

                //如果pointsPerHolding还有剩余的点，移除首个（i.e.下落操作）
                if (points.Count > 0)
                    pointsPerHolding.RemoveAt(0);

                //对于pointsPerHolding内每一个点：
                float nowDivide = 0;
                for (int j = 0; j < pointsPerHolding.Count; j++)
                {
                    //取得当前点下落的方向
                    KeyValuePair<Vector2, Vector2> normalPerPoint =
                        JudgementLine.CurrentCurve.GetNormal(start + divide[j] - nowDivide);
                    //计算当前点将要下落的绝对位置，并将当前点更新到那个位置
                    pointsPerHolding[j] =
                        StartPoint +
                        PositionHelper.RelativeCoordToAbsoluteCoord(pointsPerHolding[j], Camera.main) +
                        perDirection * (j + 1) * normalPerPoint.Value;
                    nowDivide = divide[j];
                }

                //更新points，(p.s. points就是要渲染的点组)
                //如果当前时间在当前分段内
                if (CurrentTime > time && CurrentTime <= time + Holding.Time) {
                    //当前引导线相对起始时间的行进进度（百分率）（i.e.下落时间进度）
                    float progress = (CurrentTime - time) / Holding.Time;
                    //当前引导线相对起始位置的行进位置（i.e.下落空间进度）
                    float easedProgress = EaseTypeManager.GetEaseValue(progress, Holding.EaseType);

                    //现在行进到第几个Point?以时间进度计算
                    float nowPointPosition = progress * (pointsPerHolding.Count - 1);
                    //计算渲染点组起始下标位置
                    int nowPointIndex = Mathf.FloorToInt(nowPointPosition);
                    //计算小数部分
                    float nowPointPositionDigit = nowPointPosition - nowPointIndex;
                    //连接小数部分
                    //TODO:目前是线性连接，待大佬修改为缓动曲线连接
                    if (pointsPerHolding.Count - 1 > nowPointIndex)
                    {
                        pointsPerHolding[nowPointIndex] += (pointsPerHolding[nowPointIndex + 1] - pointsPerHolding[nowPointIndex]) * nowPointPositionDigit;

                    }
                    //计算在progress处点的位置，并赋值给position
                    position = start + (Holding.To - start) * easedProgress;
                    Debug.Log($"{position}");
                    //将从index起始的pointsPerHolding点组添加到points中
                    points.AddRange(pointsPerHolding.GetRange(nowPointIndex, pointsPerHolding.Count - nowPointIndex));
                }
                //如果当前时间在当前分段起始时间之前
                else if (CurrentTime <= time) {
                    //如果pointsPerHolding的第一个点与points的最后一个点位置相同，则移除pointsPerHolding的第一个点(i.e.连接应相连的引导线)
                    if (points.Count > 0 && points[^1] == pointsPerHolding[0])
                        pointsPerHolding.RemoveAt(0);
                    //将pointsPerHolding直接添加到points中
                    points.AddRange(pointsPerHolding);
                }

                //更新起点与终点时间
                start = Holding.To;
                time += Holding.Time;
                //将起始点设置为当前分曲线的最后一个点
                StartPoint = pointsPerHolding[^1];
            }
            
            //渲染线(根据points)
            if (points.Count > 0)
            {
                // Line.transform.localPosition.y = -points[0].y;
                var pos = new Vector3((-points[0]).x, (-points[0]).y);
                Line.transform.SetLocalPositionAndRotation(pos, Quaternion.identity);
            }
            Line.positionCount = points.Count;
            Line.SetPositions(Functions.Vec2ListToVec3List(points).ToArray());
            Line.startColor = new Color(0, 0.8f, 0, 0.8f);
            Line.endColor = new Color(1, 0, 0, 0.8f);
            Line.startWidth = 0.12f;
            Line.endWidth = 0.12f;
            //设置音符位置
            KeyValuePair<Vector2, Vector2> normal = JudgementLine.CurrentCurve.GetNormal(position);
            //计算判定点相对判定线的位置
            transform.localPosition = PositionHelper.RelativeCoordToAbsoluteCoord(normal.Key, Camera.main) + 
                      (CurrentTime < ArrivalTime ? normal.Value * JudgementLine.Speed.GetPosition(CurrentTime, ArrivalTime - CurrentTime) : Vector2.zero);
            


            // Renderer.color = Line.startColor = Line.endColor = NoteManager.NoteColor;
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
