using Dremu.Gameplay.Object;
using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Manager;
using Dremu.Gameplay.Tool;
using UnityEngine;
using Utils.Helper;

namespace Dremu.Gameplay.Object {

    public sealed class Slide : NoteBase {
        [SerializeField] LineRenderer Line;
        private float Width;

        public override void OnActive( float CurrentTime ) {
            //实时更新形状
            // var points = new List<Vector2>(JudgmentLine.CurrentCurve.SubCurveByMidAndLength(position, Width));
            // for (int i = 0; i < points.Count; i++)
            //     points[i] = PositionHelper.RelativeCoordToAbsoluteCoord(points[i], Camera.main);
            //
            // Line.positionCount = points.Count;
            // Line.SetPositions(Functions.Vec2ListToVec3List(points).ToArray());

            //设置音符位置
            KeyValuePair<Vector2, Vector2> normal = JudgmentLine.CurrentCurve.GetNormal(position);
            transform.localPosition = PositionHelper.RelativeCoordToAbsoluteCoord(normal.Key, Camera.main) + normal.Value * JudgmentLine.Speed.GetPosition(CurrentTime, ArrivalTime - CurrentTime);

            if (CurrentTime > ArrivalTime) //淡出
                Line.startColor = Line.endColor = UGUIHelper.SetAlpha(NoteManager.NoteColor, 1 - (CurrentTime - ArrivalTime)*2);
        }

        public override void OnInitialize() {

        }

        public override void OnRecycle() {

        }


        /// <summary>
        /// 设置音符宽度
        /// </summary>
        /// <param name="Width">宽度</param>
        public void SetWidth( float Width ) {
            this.Width = Width;
        }
    }

}
