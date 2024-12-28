#nullable enable
#define DEBUG


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using Dremu.Gameplay.Object;
using Dremu.Gameplay.Tool;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Curve = Dremu.Gameplay.Tool.Curve;
using EnvelopeLine = Dremu.Gameplay.Tool.EnvelopeLine;
using GuideLine = Dremu.Gameplay.Object.GuideLine;
using JudgementLine = Dremu.Gameplay.Object.JudgementLine;


/*~qwfdhn*/
namespace Dremu.Gameplay.Manager
{
    public class MainController : MonoBehaviour
    {
        [SerializeField, Min(0)] float CurrentTime;

        [SerializeField] private Button pauseButton;
        [SerializeField] private Text songName;
        [SerializeField] private Text hard;

        static MainController Instance;

        public delegate void Callback();

        public string jsonPath;
        private static bool isPaused = false;
        public Utils.Helper.ChartHelper.Root chart;


        public static void Stop(Callback? callback)
        {
            Time.timeScale = 0;
            AudioManager.StopMusic();
            isPaused = true;
            Debug.Log("Using Pause Button");
            if (callback != null)
            {
                callback();
            }
        }


        public static void ContinueGame(Callback? callback)
        {
            Time.timeScale = 1;
            AudioManager.UnPauseMusic();
            isPaused = false;
            Debug.Log("Using Continue Button");
            if (callback != null)
            {
                callback();
            }
        }

        public static void RestartGame(Callback? callback)
        {
            SceneManager.LoadScene(0);
            if (callback != null)
            {
                callback();
            }
        }
        
        
        private static Curve CurveFromChart(Utils.Helper.ChartHelper.Curve chartCurve)
        {
            return new Curve(
                chartCurve.Points,
                chartCurve.Nodes
            );
        }
        
        private static JudgementLine LineFromChart(Curve currentCurve, Utils.Helper.ChartHelper.JudgementLine chartLine)
        {
            return JudgmentLineManager.GetNewJudgmentLine(currentCurve, new EnvelopeLine(
                   new List<float[]>()
                   {
                       new float[]
                       {
                           0, 5
                       }
                   }
                   ), new EnvelopeLine(
                   new List<float[]>()
                   {
                       new float[]
                       {
                           0, 1
                       }
                   }
                   ), 0.15f);
            //TODO:这个0，5是个啥
        }
        
        public static CurveType getCurveType(string curveType)
        {
            return curveType switch
            {
                "Const" => CurveType.Const,
                "Linear" => CurveType.Linear,
                "Expo" => CurveType.Expo,
                "Sine" => CurveType.Sine,
                _ => CurveType.Const
            };
        }
        private static ControlNode ConvertController(Utils.Helper.ChartHelper.Controller chartController)
        {
            return new ControlNode(chartController.Time, chartController.Value, chartController.Tension, getCurveType(chartController.CurveType) );
        }
        private static EnvelopeLine EnvelopeLineFromChart(Utils.Helper.ChartHelper.EnvelopeLine chartEnvelopeLine)
        {
            var t = new EnvelopeLine(new List<ControlNode> { });
            foreach (var c in chartEnvelopeLine.Controllers)
            {
                t.Controllers.Add(ConvertController(c));
            }
            return t;
        }

        private static void createNote(JudgementLine chartLine, Utils.Helper.ChartHelper.Note chartNote)
        {
            switch (chartNote.Type)
            {
                case "Tap": NoteManager.GetNewTap(chartLine, chartNote.Position, chartNote.ArrivalTime); break;
                case "Slide": NoteManager.GetNewSlide(chartLine, chartNote.Position, chartNote.ArrivalTime); break;
                case "Hold": throw new NotImplementedException("Hold"); break;
                case "Drag": throw new NotImplementedException("Drag"); break;
                default: throw new Exception("Note类型不正确！");
            };
        }

        private static List<GuideLine.GuideNode> convertNodes(Utils.Helper.ChartHelper.GuideLine chartGuide)
        {
            List<GuideLine.GuideNode> t = new List<GuideLine.GuideNode>();
            foreach (var chartNode in chartGuide.Nodes)
            {
                t.Add(new GuideLine.GuideNode(chartNode.To, chartNode.Time, EaseTypeManager.EaseType.LINEAR));
                //TODO:缓动类型我真的不想再switch了！！！！！！！
            }
            return t;
        }
        private static void createGuideLine(JudgementLine chartLine,Utils.Helper.ChartHelper.GuideLine chartGuide)
        {
            NoteManager.GetNewGuideLine(chartLine, chartGuide.Position, chartGuide.ArrivalTime,
                convertNodes(chartGuide));

        }
        //TODO:能改到ChartHelper里就改
        
#if DEBUG
        //设置默认的Bpm
        EnvelopeLine BPMLine=new EnvelopeLine(new List<ControlNode>()
        {
            new ControlNode(0, 120, 0, CurveType.Linear)
        }); //BpmList Linear是变化形式，Const是直接把BPM改成对应的数值

        public static EnvelopeLine BPM
        {
            get { return Instance.BPMLine; }
            set { Instance.BPMLine = value; }
        }


        JudgementLine line;
#endif
        [Obsolete("Obsolete")]
        private void Start()
        {
            pauseButton.onClick.AddListener(() => { RestartGame(null); });
            jsonPath = "Assets/Resources/Chart/TestChart_1.json";

            // JObject chart = ChartAnalyser.GetChartDataToJObject(jsonPath);
            chart = Utils.Helper.ChartHelper.ChartAnalyser.GetChartDataToRoot(jsonPath);

            if (chart == null) throw new NullReferenceException();

            //谱面头部信息
            if (chart.Version != "2024.8.7" || chart.Version == null)
            {
                throw new Exception("版本不匹配");
                Application.Quit();
            }

            songName.text = chart.Name;
            hard.text = chart.Hard;

            #region SetDefaultBPM

            if (chart.ChartData.BPMList == null)
            {
                BPM = new EnvelopeLine(new List<ControlNode>()
                {
                    new ControlNode(0, chart.DefaultBPM, 0, CurveType.Linear),
                });
            }
            else
            {
                List<ControlNode> bpmList = new List<ControlNode>();
                foreach (var bpm in chart.ChartData.BPMList)
                {
                    Debug.Log(bpm.Time);
                    bpmList.Add(new ControlNode(bpm.Time, bpm.Value, bpm.Tension, EaseTypeManager.StringToCurveType(bpm.CurveType)));
                }
                BPM = new EnvelopeLine(bpmList);
            }

            #endregion

            foreach (Utils.Helper.ChartHelper.JudgementLine judgementLine in chart.ChartData.JudgementLineList)
            {
                //TODO:对复杂谱面多个判定线的遍历，下次再处理这个
            }

            AudioManager.PlayMusic(clip);
            AudioManager.MusicVolume = 1;
#if DEBUG

            Curve curve1 = CurveFromChart(chart.ChartData.JudgementLineList[0].CurveGroup[0]);
            Curve curve2 = CurveFromChart(chart.ChartData.JudgementLineList[0].CurveGroup[1]);
            line = LineFromChart(curve1, chart.ChartData.JudgementLineList[0]);
            var curveGroup = new List<Curve>
            {
                curve1,
                curve2
            };
            var ev = EnvelopeLineFromChart(chart.ChartData.JudgementLineList[0].EnvelopeLine);
            line.SetCurvesAndEnvelope(curveGroup, ev);


            createNote(line,chart.ChartData.JudgementLineList[0].Note[0]);
            createNote(line,chart.ChartData.JudgementLineList[0].Note[1]);
            
            createGuideLine(line,chart.ChartData.JudgementLineList[0].GuideLine[0]);
            
            NoteManager.GetNewDrag(line, 0.5f, 17, new List<Hold.HoldNode>()
            {
                new Hold.HoldNode(0.3f, 1),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.7f, 2),
            });
        }
#endif
        // #endregion

        [SerializeField] float defaultBpm;

        public AudioClip clip;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            JudgmentLineManager.UpdateJudgmentLineState(CurrentTime);
            NoteManager.UpdateNoteState(CurrentTime);
            NoteEffectManager.UpdateNoteEffectState();


            CurrentTime = BPM.GetBeatFromSecond(AudioManager.Time);
        }
    }
}