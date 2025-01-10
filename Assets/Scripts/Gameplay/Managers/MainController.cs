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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Curve = Dremu.Gameplay.Tool.Curve;
using EnvelopeLine = Dremu.Gameplay.Tool.EnvelopeLine;
using GuideLine = Dremu.Gameplay.Object.GuideLine;
using JudgementLine = Dremu.Gameplay.Object.JudgementLine;
using Utils.Helper.ChartConvertHelper;


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
        public Utils.Helper.ChartConvertHelper.Chart chart;


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
        
        
        
        public static void CreateNotes(JudgementLine chartLine, Utils.Helper.ChartHelper.Notes chartNotes)
        {
            foreach (var tap in chartNotes.Taps)
            {
                NoteManager.GetNewTap(chartLine, tap.Position, tap.ArrivalTime);
            }
            foreach (var slide in chartNotes.Slides)
            {
                NoteManager.GetNewSlide(chartLine, slide.Position, slide.ArrivalTime);
            }
        }
        
        public static void CreateGuideLine(JudgementLine chartLine,Utils.Helper.ChartConvertHelper.GuideLine chartGuide)
        {
            NoteManager.GetNewGuideLine(chartLine, chartGuide.Position, chartGuide.ArrivalTime,
                ChartConvertHelper.ConvertNodes(chartGuide.Nodes));

        }
        
        private static JudgementLine LineFromChart(Curve currentCurve, Utils.Helper.ChartConvertHelper.JudgementLine chartLine)
        {
            return JudgmentLineManager.GetNewJudgmentLine(currentCurve, chartLine.Speed, chartLine.Alpha, 0.15f);
        }
        //?
        
#if DEBUG
        //设置默认的Bpm
        EnvelopeLine BPMLine=new EnvelopeLine(new List<ControllNode>()
        {
            new ControllNode(0, 120, 0, CurveType.Linear)
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
            chart = ChartConvertHelper.ConvertChartFromRoot(Utils.Helper.ChartHelper.ChartAnalyser.GetChartDataToRoot(jsonPath));

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
                BPM = new EnvelopeLine(new List<ControllNode>()
                {
                    new ControllNode(0, chart.DefaultBPM, 0, CurveType.Linear),
                });
            }
            else
            {
                List<ControllNode> bpmList = new List<ControllNode>();
                foreach (var bpm in chart.ChartData.BPMList)
                {
                    Debug.Log(bpm.Time);
                    bpmList.Add(new ControllNode(bpm.Time, bpm.Value, bpm.Tension, EaseTypeManager.StringToCurveType(bpm.CurveType)));
                }
                BPM = new EnvelopeLine(bpmList);
            }

            #endregion

            foreach (Utils.Helper.ChartConvertHelper.JudgementLine judgementLine in chart.ChartData.JudgementLineList)
            {
                //TODO:对复杂谱面多个判定线的遍历，下次再处理这个
            }

            AudioManager.PlayMusic(clip);
            AudioManager.MusicVolume = 1;
#if DEBUG

            Curve curve1 = chart.ChartData.JudgementLineList[0].Curves[0];
            Curve curve2 = chart.ChartData.JudgementLineList[0].Curves[1];
            line = LineFromChart(curve1, chart.ChartData.JudgementLineList[0]);
            var curveGroup = new List<Curve>
            {
                curve1,
                curve2
            };
            var ev = chart.ChartData.JudgementLineList[0].CurveController;
            line.SetCurvesAndEnvelope(curveGroup, ev);


            CreateNotes(line,chart.ChartData.JudgementLineList[0].Notes);
            
            CreateGuideLine(line,chart.ChartData.JudgementLineList[0].GuideLines[0]);
            
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