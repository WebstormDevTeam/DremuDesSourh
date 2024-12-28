using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Manager;
using Dremu.Gameplay.Tool;
using UnityEngine;
using Utils.Helper;

namespace Dremu.Gameplay.Object {
    public class JudgementLine : MonoBehaviour, RecyclableObject {

        [SerializeField] LineRenderer line;
        
        /// <summary>
        /// 当前曲线
        /// </summary>
        public Curve CurrentCurve { get; private set; }
        /// <summary>
        /// 曲线列表
        /// </summary>
        public List<Curve> Curves { get; private set; }
        /// <summary>
        /// 曲线控制组
        /// </summary>
        public EnvelopeLine CurveControler {get; private set; }
        /// <summary>
        /// 流速控制组
        /// </summary>
        public EnvelopeLine Speed { get; private set; }
        /// <summary>
        /// 透明度控制组
        /// </summary>
        public EnvelopeLine Alpha { get; private set; }
        /// <summary>
        /// 判定线上音符列表
        /// </summary>
        public List<NoteBase> Notes = new List<NoteBase>();
        /// <summary>
        /// 判定线上音符的宽度
        /// </summary>
        public float NoteWidth { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInitialize() {
            line.positionCount = 0;
            CurrentCurve = null;
        }
        /// <summary>
        /// 回收
        /// </summary>
        public void OnRecycle() {
            Notes.Clear();
        }
        /// <summary>
        /// 添加音符
        /// </summary>
        /// <param name="Note">Note</param>
        public void AddNote( NoteBase Note ) {
            Note.Bind(this);
            Notes.Add(Note);
        }

        /// <summary>
        /// 活动时调用
        /// 判定线管理器会自动调用，谁敢调用他我跟谁急奥！
        /// </summary>
        /// <param name="CurrentTime">当前时间</param>
        public void OnActive( float CurrentTime ) {
            //实时更新形状
            UpdateCurve(CurrentTime);

            List<Vector2> points = new List<Vector2>(CurrentCurve.GetPoints());
            for (int i = 0; i < points.Count; i++)
                points[i] = PositionHelper.RelativeCoordToAbsoluteCoord(points[i], Camera.main);
            line.positionCount = points.Count;
            line.SetPositions(Functions.Vec2ListToVec3List(points).ToArray());
            line.startColor = UGUIHelper.SetAlpha(JudgmentLineManager.JudgmentLineColor, Alpha.GetValue(CurrentTime));
            line.endColor = UGUIHelper.SetAlpha(JudgmentLineManager.JudgmentLineColor, Alpha.GetValue(CurrentTime));
        }

        /// <summary>
        /// 设置曲线和控制组
        /// </summary>
        /// <param name="curves">曲线</param>
        /// <param name="envelopeLine">控制组</param>
        public void SetCurvesAndEnvelope( List<Curve> curves, EnvelopeLine envelopeLine ) {
            Curves = curves;
            CurveControler = envelopeLine;
            CurrentCurve = curves[0];
        }

        /// <summary>
        /// 更新曲线
        /// </summary>
        /// <param name="CurrentTime">当前时间</param>
        /// <exception cref="Exception">曲线控制组为空</exception>
        private void UpdateCurve(float CurrentTime)
        {
            int curveIndex = EnvelopeLine.GetControlerIndex(CurveControler.Controllers, CurrentTime);
            if (curveIndex == -1)
            {
                throw new System.Exception("Error when search the time, is the list empty?");
            }
            if (curveIndex == CurveControler.Controllers.Count - 1)
            {
                // 不对curve做出变化
                return;
            }
            var startIndex = Mathf.FloorToInt(CurveControler.Controllers[curveIndex].Value);
            var endIndex = Mathf.FloorToInt(CurveControler.Controllers[curveIndex + 1].Value);
            var startCurve = Curves[startIndex];
            var endCurve = Curves[endIndex];
            var progress = (startIndex == endIndex)?  1f:(CurveControler.GetValue(CurrentTime) - endIndex) / (startIndex - endIndex);
            CurrentCurve = Curve.CurveLerp(startCurve, endCurve, progress);
        }
        

        /// <summary>
        /// 设置流速组
        /// </summary>
        /// <param name="SpeedGroup">流速组</param>
        public void SetSpeedGroup(EnvelopeLine SpeedGroup ) {
            Speed = SpeedGroup;
        }


        /// <summary>
        /// 设置透明度组
        /// </summary>
        /// <param name="SpeedGroup">透明度组</param>
        public void SetAlphaGroup(EnvelopeLine AlphaGroup)
        {
            Alpha = AlphaGroup;
        }

        /// <summary>
        /// 设置判定线上音符的宽度
        /// </summary>
        /// <param name="NoteWidth">宽度</param>
        public void SetNoteWidth(float NoteWidth ) {
            this.NoteWidth = NoteWidth;
        }

    }


}
