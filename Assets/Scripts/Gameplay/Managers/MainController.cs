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
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Helper.ChartHelper;
using Curve = Dremu.Gameplay.Tool.Curve;
using EnvelopeLine = Dremu.Gameplay.Tool.EnvelopeLine;
using GuideLine = Dremu.Gameplay.Object.GuideLine;


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

#if DEBUG
        EnvelopeLine BPMLine=new EnvelopeLine(new List<ControlNode>()
        {
            new ControlNode(0, 120, 0, CurveType.Linear)
        }); //BpmList Linear是变化形式，Const是直接把BPM改成对应的数值

        public static EnvelopeLine BPM
        {
            get { return Instance.BPMLine; }
            set { Instance.BPMLine = value; }
        }

        // #region test


        Curve curve1 = new Curve(
                new List<Vector2>()
                {
                    new Vector2(-0.8f, 0.0f),
                    new Vector2(0.8f, 0.0f)
                },
                new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2(0f, 0f)
                    }
                }
            ),
            curve2 = new Curve(
                new List<Vector2>()
                {
                    new Vector2(-0.8f, -1),
                    new Vector2(0.8f, -1),
                },
                new List<List<Vector2>>
                {
                    new List<Vector2>
                    {
                        new Vector2(-0.5f, 0),
                        new Vector2(0.5f, 0)
                    }
                }
            );

        JudgmentLine line;
#endif
        [Obsolete("Obsolete")]
        private void Start()
        {
            pauseButton.onClick.AddListener(() => { RestartGame(null); });
            jsonPath = "Assets/Resources/Chart/TestChart_1.json";

            JObject chart = ChartAnalyser.GetChartDataToJObject(jsonPath);

            //谱面头部信息
            if (chart["Version"].ToString() != "2024.8.7" || chart["Version"] == null)
            {
                throw new Exception("版本不匹配");
                Application.Quit();
            }

            songName.text = chart["Name"].ToString();
            hard.text = chart["Hard"].ToString();

            #region SetDefaultBPM

            if (chart["ChartData"]["BPMList"] == null)
            {
                BPM = new EnvelopeLine(new List<ControlNode>()
                {
                    new ControlNode(0, (int)chart["DefaultBPM"], 0, CurveType.Linear),
                });
            }
            else
            {
                List<ControlNode> bpmList = new List<ControlNode>();
                foreach (JObject bpm in chart["ChartData"]["BPMList"].ToArray())
                {
                    Debug.Log(bpm["Time"]);
                    bpmList.Add(new ControlNode((int)bpm["Time"], (int)bpm["Value"], (int)bpm["Tension"], EaseTypeManager.StringToCurveType(bpm["CurveType"].ToString())));
                }
                BPM = new EnvelopeLine(bpmList);
            }

            #endregion

            AudioManager.PlayMusic(clip);
            AudioManager.MusicVolume = 1;
#if DEBUG
            line = JudgmentLineManager.GetNewJudgmentLine(curve1, new EnvelopeLine(
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
            var curveGroup = new List<Curve>
            {
                curve1,
                curve2
            };
            var ev = new EnvelopeLine(
                new List<ControlNode>
                {
                    new ControlNode(
                        0, 0, 1, CurveType.Expo
                    ),
                    new ControlNode(
                        20, 1, 1, CurveType.Const
                    )
                }
            );
            line.SetCurvesAndEnvelope(curveGroup, ev);


            for (int i = 0; i < 16; i++)
            {
                NoteManager.GetNewTap(line, 0.2f, 15f + i * 2);
                NoteManager.GetNewSlide(line, 0.4f, 15.5f + i * 2);
                NoteManager.GetNewTap(line, 0.6f, 16f + i * 2);
                NoteManager.GetNewSlide(line, 0.8f, 16.5f + i * 2);
            }

            ///*
            NoteManager.GetNewTap(line, 0.5f, 20f);
            NoteManager.GetNewSlide(line, 0.5f, 17);


            for (int i = 0; i < 13; i++)
            {
                // NoteManager.GetNewGuideLine(line, 0.8f, 18 + i * 4, new List<GuideLine.GuideNode>()
                // {
                //     new GuideLine.GuideNode(0.5f, 1.5f, EaseTypeManager.EaseType.LINEAR),
                //     new GuideLine.GuideNode(0.6f, 1.5f, EaseTypeManager.EaseType.LINEAR)
                // });
                // NoteManager.GetNewSlide(line, 0.5f, 19.5f + i * 4);
                // NoteManager.GetNewGuideLine(line, 0.2f, 20 + i * 4, new List<GuideLine.GuideNode>()
                // {
                //     new GuideLine.GuideNode(0.2f, 1.5f, EaseTypeManager.EaseType.LINEAR),
                //     new GuideLine.GuideNode(0.6f, 1.5f, EaseTypeManager.EaseType.LINEAR)
                // });
                NoteManager.GetNewSlide(line, 0.2f, 21.5f + i * 4);
            }

            NoteManager.GetNewGuideLine(line, 0.5f, 17, new List<GuideLine.GuideNode>()
            {
                new GuideLine.GuideNode(0.7f, 1, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.EASE_IN_QUAD),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.EASE_IN_CIRC),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.EASE_IN_OUT_SINE),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.EASE_IN_OUT_BOUNCE),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.EASE_IN_CIRC),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.7f, 2, EaseTypeManager.EaseType.LINEAR),
                new GuideLine.GuideNode(0.3f, 2, EaseTypeManager.EaseType.LINEAR),
            });

            //
            // NoteManager.GetNewDrag(line, 0.5f, 17, new List<Hold.HoldNode>()
            // {
            //     new Hold.HoldNode(0.3f, 1),
            //     new Hold.HoldNode(0.4f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.4f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.5f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.2f, 2),
            //     new Hold.HoldNode(0.8f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.9f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.5f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            //     new Hold.HoldNode(0.3f, 2),
            //     new Hold.HoldNode(0.7f, 2),
            // });
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