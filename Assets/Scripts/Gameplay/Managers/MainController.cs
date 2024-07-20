using System;
using System.Collections.Generic;
using Dremu.Gameplay.Object;
using Dremu.Gameplay.Tool;
using UnityEngine;

namespace Dremu.Gameplay.Manager
{
    public class MainController : MonoBehaviour
    {
        EnvelopeLine BPMLine = new EnvelopeLine(new List<ControlNode>()
        {
            new ControlNode(0, 184, 0, CurveType.Linear),
            new ControlNode(10, 184, 0, CurveType.Const)
        });

        [SerializeField, Min(0)] float CurrentTime;

        static MainController Instance;

        public static EnvelopeLine BPM
        {
            get => Instance.BPMLine;
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

        [Obsolete("Obsolete")]
        private void Start()
        {
            AudioManager.PlayMusic(clip);
            AudioManager.MusicVolume = 1;

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

            #region ??????

            // for (int i = 0; i < 16; i++)
            // {
            //     NoteManager.GetNewTap(line, 0.2f, 15f + i * 2);
            //     NoteManager.GetNewSlide(line, 0.4f, 15.5f + i * 2);
            //     NoteManager.GetNewTap(line, 0.6f, 16f + i * 2);
            //     NoteManager.GetNewSlide(line, 0.8f, 16.5f + i * 2);
            // }

            ///*
            NoteManager.GetNewTap(line, 0.5f, 20f); 
            NoteManager.GetNewSlide(line, 0.5f, 17);

            #endregion

            for (int i = 0; i < 13; i++)
            {
                NoteManager.GetNewHold(line, 0.8f, 18 + i * 4, new List<GuideLine.GuideNode>()
                {
                    new GuideLine.GuideNode(0.5f, 1.5f)
                });
                NoteManager.GetNewSlide(line, 0.5f, 19.5f + i * 4);
                NoteManager.GetNewHold(line, 0.2f, 20 + i * 4, new List<GuideLine.GuideNode>()
                {
                    new GuideLine.GuideNode(0.2f, 1.5f)
                });
                NoteManager.GetNewSlide(line, 0.2f, 21.5f + i * 4);
            }

            NoteManager.GetNewHold(line, 0.5f, 17, new List<GuideLine.GuideNode>()
            {
                new GuideLine.GuideNode(0.7f, 1),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
                new GuideLine.GuideNode(0.7f, 2),
                new GuideLine.GuideNode(0.3f, 2),
            });

            #region ???????????????????????

            NoteManager.GetNewDrag(line, 0.5f, 17, new List<Hold.HoldNode>()
            {
                new Hold.HoldNode(0.3f, 1),
                new Hold.HoldNode(0.4f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.4f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.5f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.3f, 2),
                new Hold.HoldNode(0.2f, 2),
                new Hold.HoldNode(0.8f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.9f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.7f, 2),
                new Hold.HoldNode(0.5f, 2),
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

            #endregion
        }

        // #endregion

        [SerializeField] float bpm;

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