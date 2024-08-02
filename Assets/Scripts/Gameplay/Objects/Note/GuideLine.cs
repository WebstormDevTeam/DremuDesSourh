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
            
            //points：所有点的集合
            var points = new List<Vector2>();
            //初始化起点，以及到达判定线的时间
            float start = this.position;
            float time = ArrivalTime;
            Vector2 StartPoint = Vector2.zero;

            EaseTypeEnumer.EaseType nowEaseType = EaseTypeEnumer.EaseType.LINEAR;
            //对于当前引导线：
            for (int i = 0; i < GuideLineNodes.Count; i++) {
                GuideNode Holding = GuideLineNodes[i];
                //利用新函数将使得选取的曲线呈现缓动函数形态
                var pointsPerHolding = JudgmentLine.CurrentCurve.SubCurveByStartAndEnd(start, Holding.To, nowEaseType);
                
                ////////////////TODO:对于点下落的计算，目前是线性插值，需要改成曲线插值，目前尚未完善
                
                // //计算相对于起点，每个点下落空间位置的微分单位
                // // float divide = 1f * (Holding.To - start) / pointsPerHolding.Count;
                // //相对于起点，每个点下落时间位置的微分单位
                // float perDirection = JudgmentLine.Speed.GetPosition(time, Holding.Time) / pointsPerHolding.Count;
                
                ////////尝试性改动：将其设置为曲线插值////////
                //计算相对于起点，每个点下落空间(横轴)的位置
                List<float> divide = EaseTypeEnumer.GetEaseLine(Holding.To - start, pointsPerHolding.Count, nowEaseType);
                //相对于起点，每个点下落时间(纵轴)的位置变化量
                float perDirection = JudgmentLine.Speed.GetPosition(time, Holding.Time) / pointsPerHolding.Count;
                // List<float> perDirection = EaseTypeEnumer.GetEase(
                //     JudgmentLine.Speed.GetPosition(time, Holding.Time),
                //     pointsPerHolding.Count, nowEaseType);
                

                // //如果pointsPerHolding还有剩余的点，移除首个（i.e.线性下落操作）
                // if (points.Count > 0)
                //     pointsPerHolding.RemoveAt(0);
                //如果pointsPerHolding还有剩余的点，移除应被移除的（i.e.缓动下落操作）
                while (points.Count > 0 && points[0].y <= 0) 
                    pointsPerHolding.RemoveAt(0);

                //对于pointsPerHolding内每一个点：
                float nowDivide = 0;
                for (int j = 0; j < pointsPerHolding.Count; j++)
                {
                    //取得当前点下落后的法线
                    KeyValuePair<Vector2, Vector2> normalPerPoint =
                        JudgmentLine.CurrentCurve.GetNormal(start + divide[j] - nowDivide);
                    //计算当前点将要下落的绝对位置，并将当前点更新到那个位置
                    pointsPerHolding[j] =
                        StartPoint +
                        PositionHelper.RelativeCoordToAbsoluteCoord(pointsPerHolding[j], Camera.main) +
                        perDirection * (j + 1) * normalPerPoint.Value;
                    nowDivide = divide[j];
                }
                ////////////////

                //更新points，(p.s. points就是要渲染的点组)
                //如果当前时间在当前分段内
                if (CurrentTime > time && CurrentTime <= time + Holding.Time) {
                    //当前引导线相对起始时间的行进进度（百分率）（i.e.下落进度）
                    float progress = (CurrentTime - time) / Holding.Time;

                    //现在行进到第几个Points了?
                    float nowPointPosition = progress * (pointsPerHolding.Count - 1);  //TODO:我想要在这里写一个Ease但是我不知道怎么写啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊
                    //计算渲染点组起始下标位置
                    int nowPointIndex = Mathf.FloorToInt(nowPointPosition);
                    //计算小数部分
                    float nowPointPositionDigit = nowPointPosition - nowPointIndex;
                    //线性连接小数部分，使之看起来更平顺
                    if (pointsPerHolding.Count - 1 > nowPointIndex)
                        pointsPerHolding[nowPointIndex] += (pointsPerHolding[nowPointIndex + 1] - pointsPerHolding[nowPointIndex]) * nowPointPositionDigit;
                    
                    position = start + (Holding.To - start) * progress;
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
            if (points.Count > 0) Line.transform.localPosition = -points[0];
            Line.positionCount = points.Count;
            Line.SetPositions(Functions.Vec2ListToVec3List(points).ToArray());
            Line.startColor = new Color(0, 0.8f, 0, 0.8f);
            Line.endColor = new Color(1, 0, 0, 0.8f);
            Line.startWidth = 0.08f;
            Line.endWidth = 0.08f;
            //设置音符位置
            KeyValuePair<Vector2, Vector2> normal = JudgmentLine.CurrentCurve.GetNormal(position);
            transform.localPosition = 
                PositionHelper.RelativeCoordToAbsoluteCoord(normal.Key, Camera.main) + 
                (CurrentTime < ArrivalTime ? normal.Value * JudgmentLine.Speed.GetPosition(CurrentTime, ArrivalTime - CurrentTime) : Vector2.zero);

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
