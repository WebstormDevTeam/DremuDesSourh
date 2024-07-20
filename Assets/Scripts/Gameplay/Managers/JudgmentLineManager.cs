using System.Collections;
using System.Collections.Generic;
using Dremu.Gameplay.Object;
using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.Gameplay.Manager {
    public sealed class JudgmentLineManager : MonoBehaviour {

        [SerializeField] JudgmentLine _JudgmentLine;

        private StandardObjectPool<JudgmentLine> JudgmentLinePool;
        public static JudgmentLineManager Instance { get; private set; }

        readonly List<JudgmentLine> ActiveLines = new List<JudgmentLine>();

        public static Color JudgmentLineColor { get; private set; }

        private void Awake() {
            Instance = this;

            JudgmentLinePool = new StandardObjectPool<JudgmentLine>(_JudgmentLine, 4);
            JudgmentLineColor = Color.black;
        }

        /// <summary>
        /// 获取一条判定线
        /// </summary>
        /// <param name="InitialCurve">初始曲线</param>
        /// <param name="SpeedGroup">流速组</param>
        /// <param name="AlphaGroup">透明度组</param>
        /// <param name="NoteWidth">音符宽度</param>
        /// <returns>一条判定线</returns>
        public static JudgmentLine GetNewJudgmentLine(Curve InitialCurve, EnvelopeLine SpeedGroup, EnvelopeLine AlphaGroup, float NoteWidth) {
            JudgmentLine line = Instance.JudgmentLinePool.GetObject();
            line.SetCurvesAndEnvelope(
                new List<Curve> { InitialCurve },
                new EnvelopeLine(
                    new List<ControlNode> {
                        new ControlNode(
                            0,0,1,CurveType.Sine
                        )
                    }
                ));
            line.SetSpeedGroup(SpeedGroup);
            line.SetAlphaGroup(AlphaGroup);
            line.SetNoteWidth(NoteWidth);
            Instance.ActiveLines.Add(line);
            return line;
        }
        /// <summary>
        /// 归还一条判定线
        /// </summary>
        /// <param name="JudgmentLine"> 判定线对象 </param>
        public static void ReturnJudgmentLine(JudgmentLine JudgmentLine ) {
            Instance.JudgmentLinePool.ReturnObject(JudgmentLine);
        }

        /// <summary>
        /// 更新判定线状态
        /// </summary>
        /// <param name="CurrentTime"> 当前时间 </param>
        public static void UpdateJudgmentLineState(float CurrentTime) {
            foreach (var line in Instance.ActiveLines) {
                line.OnActive(CurrentTime);
            }
        }

    }
}
