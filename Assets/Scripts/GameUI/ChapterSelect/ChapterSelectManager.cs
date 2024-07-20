using System;
using System.Linq;
using Data;
using GameUI.Global.ColorSet;
using GameUI.Global.Sound;
using GameUI.Global.Transition;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Utils.Singleton;

namespace GameUI.ChapterSelect
{
    [DisallowMultipleComponent]
    public class ChapterSelectManager : MonoBehaviourSingleton<ChapterSelectManager>
    {
        [Title("预制件们")] 
        [SerializeField] private ChapterShow chapterShowPrefab;

        [Title("组件们")] 
        [SerializeField] private RectTransform chapterTransform;
        [SerializeField] private Button backButton;
        
        [Title("数值们")]
        [SerializeField, Min(0)] private float scrollSpeed;
        [SerializeField, Min(0)] private float bounceSpeed;
        [SerializeField] private AnimationCurve bounceCurve;
        [SerializeField] private AnimationCurve showOutCurve;
        
        [Title("Debug")]
        [OdinSerialize, ReadOnly] private ChapterShow[] ChapterShows { get; set; }
        public static ChapterData[] ChapterData { get; set; }
        
        private int? _nowIndex = null;
        public int NowIndex
        {
            get => _nowIndex ??= 0;
            set
            {
                value = Mathf.Clamp(value, 0, Mathf.Max(ChapterData.Length - 4, 0));
                _nowIndex = value;
                
                if (Instance)
                {
                    Instance.UpdateIndex(value);
                }
            }
        }

        private float? _scrollIndex = null;
        public float ScrollIndex
        {
            get => _scrollIndex ??= NowIndex;
            set
            {
                if (value >= -0.5f && value <= Mathf.Max(ChapterData.Length - 4, 0) + 0.5f)
                {
                    _scrollIndex = value;
                    if (Mathf.RoundToInt(value) != _nowIndex)
                    {
                        NowIndex = Mathf.RoundToInt(value);
                    }
                }
            }
        }
        
        private float? _bounceDuration = null;
        private float? _bounceStartTime = null;
        private float? _bounceStartScrollIndex = null;

        protected override void OnAwake()
        {
            backButton.onClick.AddListener(() => TransitionManager.DoScene(SceneToGo, TransitionType.DefaultWhite));
        }

        private void Start()
        {
            if (ChapterData.IsNullOrEmpty()) return;

            ChapterShows = new ChapterShow[ChapterData.Length];
            for (var i = 0; i < ChapterData.Length; i++)
            {
                var obj = Instantiate(chapterShowPrefab.gameObject, chapterTransform).GetComponent<ChapterShow>();
                obj.Init(i, ChapterData[i]);
                ChapterShows[i] = obj;
            }
            
            GetComponent<ColorSetRegister>().AddRegister(ChapterShows.SelectMany(x => x.Outlines).ToArray());

            var lastSelectedStr = PlayerPrefs.GetString("ChapterSelect_Last", null);
            var lastSelectedChapter = string.IsNullOrWhiteSpace(lastSelectedStr)
                ? ChapterData[0]
                : ChapterData.FirstOrDefault(x => x.identifier.Equals(lastSelectedStr)) ?? ChapterData[0];
            NowIndex = ChapterData.ToList().IndexOf(lastSelectedChapter);
        }

        private bool _init = false;
        private void UpdateIndex(int index)
        {
            //if (_init) UISoundManager.Play(); //TODO: 加个音效
            _init = true;
            
            PlayerPrefs.SetString("ChapterSelect_Last", ChapterData[index].identifier);
        }

        private void Update()
        {
            UpdateScrollInput();
            UpdateShowMovement();
        }

        private void UpdateScrollInput()
        {
            if (Input.touchCount > 0)
            {
                var scroll = Input.touches.Max(x => -x.deltaPosition.x);

                if (Mathf.Abs(scroll) > 1f)
                {
                    ScrollIndex += scroll * scrollSpeed / 1000f;
                }

                _bounceStartTime = null;
                _bounceStartScrollIndex = null;
                _bounceDuration = null;
            }
            else
            {
                _bounceStartTime ??= Time.time;
                _bounceStartScrollIndex ??= ScrollIndex;
                _bounceDuration ??= Mathf.Abs(ScrollIndex - NowIndex) / bounceSpeed;
                var progress = bounceCurve.Evaluate(Mathf.Clamp01((Time.time - (float) _bounceStartTime) / (float) _bounceDuration));
                ScrollIndex = Mathf.Lerp((float) _bounceStartScrollIndex, NowIndex, progress);
            }
        
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                ScrollIndex += Input.GetKey(KeyCode.RightArrow) ? 1 : -1;

                _bounceStartTime = null;
                _bounceStartScrollIndex = null;
            }
        }
        
        private void UpdateShowMovement()
        {
            if (ChapterShows.IsNullOrEmpty()) return;
            
            for (var i = 0; i < ChapterShows.Length; i++)
            {
                var pos = ChapterShows[i].transform.localPosition;
                var delta = ScrollIndex - i;
                var deltaInt = NowIndex - i;

                if (deltaInt is > 1 or < -4)
                {
                    if (ChapterShows[i].gameObject.activeSelf)
                    {
                        ChapterShows[i].gameObject.SetActive(false);
                    }
                    continue;
                }

                if (!ChapterShows[i].gameObject.activeSelf)
                {
                    ChapterShows[i].gameObject.SetActive(true);
                }

                var targetPos = -525f + delta * -1050f / 3f;
                ChapterShows[i].transform.localPosition = new Vector3(targetPos, pos.y, pos.z);

                var targetScale = delta switch
                {
                    > 0 => showOutCurve.Evaluate(Mathf.Clamp01(1f - delta)),
                    < -3 => showOutCurve.Evaluate(Mathf.Clamp01(delta + 4f)),
                    _ => 1f
                };

                ChapterShows[i].transform.localScale = new Vector3(targetScale, targetScale, 1f);
            }
        }
    }
}
