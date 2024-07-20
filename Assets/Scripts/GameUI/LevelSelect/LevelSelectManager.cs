using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using GameUI.Global.ColorSet;
using GameUI.Global.Sound;
using GameUI.Global.Transition;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Utils.Singleton;

namespace GameUI.LevelSelect
{
    [DisallowMultipleComponent]
    public class LevelSelectManager : MonoBehaviourSingleton<LevelSelectManager>
    {
        [Title("预制件们")] [SerializeField] private LevelShow levelShowPrefab;

        [Title("组件们")] [SerializeField] private Text titleText;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private RectTransform levelTransform;
        [SerializeField] private Button backButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;

        [Title("预览们")] 
        [SerializeField] private Image illustration;
        [SerializeField] private LevelShow topLevelShow;

        [Title("数值们")]
        [SerializeField, Min(0)] private float scrollSpeed;
        [SerializeField, Min(0)] private float bounceSpeed;
        [SerializeField] private AnimationCurve bounceCurve;
        [SerializeField] private AnimationCurve musicVolumeCurve;

        [Title("Debug")]
        [OdinSerialize, ReadOnly]
        public LevelShow[] LevelShows { get; private set; }


        public static ChapterData ChapterData { get; set; }
        public static LevelData[] LevelData { get; private set; }

        private int? _nowIndex = null;

        public int NowIndex
        {
            get => _nowIndex ??= 0;
            set
            {
                value = Mathf.Clamp(value, 0, Mathf.Max(LevelData.Length - 1, 0));
                _nowIndex = value;

                if (Instance)
                {
                    Instance.UpdateIndex(value, true);
                    ChangeMusicPreview(_hasScrolled);
                }
            }
        }

        private float? _scrollIndex = null;

        public float ScrollIndex
        {
            get => _scrollIndex ??= NowIndex;
            set
            {
                if (value >= -0.5f && value <= Mathf.Max(LevelData.Length - 1, 0) + 0.5f)
                {
                    _scrollIndex = value;
                    if (Mathf.RoundToInt(value) != _nowIndex)
                    {
                        NowIndex = Mathf.RoundToInt(value);
                    }
                }
            }
        }

        public static Dictionary<string, string> LastSelectedLevels // 章节名，关卡标识符
        {
            get
            {
                if (!PlayerPrefs.HasKey($"LevelSelect_LastLevels")) return null;
                var str = PlayerPrefs.GetString("LevelSelect_LastLevels");
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
            }
            set
            {
                var str = JsonConvert.SerializeObject(value, Formatting.None);
                PlayerPrefs.SetString("LevelSelect_LastLevels", str);
            }
        }

        private Coroutine _prevCoroutine;
        private float _prevStartTime;

        private float? _bounceDuration = null;
        private float? _bounceStartTime = null;
        private float? _bounceStartScrollIndex = null;

        private AudioClip[] _previewClips;

        private bool _canScroll = true;
        private bool _hasScrolled = false;

        protected override void OnAwake()
        {
            backButton.onClick.AddListener(() => TransitionManager.DoScene(SceneToGo, TransitionType.DefaultWhite));
            startButton.onClick.AddListener(StartGameplay);

            if (ChapterData == null) return;
            LevelData = ChapterData.levelData;

            titleText.text = $"{ChapterData.chapterName}: {ChapterData.subTitle}";
            
            LevelShows = new LevelShow[LevelData.Length];
            for (var i = 0; i < LevelData.Length; i++)
            {
                var obj = Instantiate(levelShowPrefab.gameObject, levelTransform).GetComponent<LevelShow>();
                obj.Init(LevelData[i], LevelSelectHarder.CurrentHarder);
                LevelShows[i] = obj;
            }
            
            GetComponent<ColorSetRegister>().AddRegister(LevelShows.Select(x => x.Outline).ToArray());
        }

        private void Start()
        {
            if (LevelData.IsNullOrEmpty()) return;

            _previewClips = new AudioClip[LevelData.Length];
            StartCoroutine(LoadPreviewClips());

            var dict = LastSelectedLevels ?? new Dictionary<string, string>();
            var levels = LevelData.Select(x => x.identifier).ToList();

            if (dict.ContainsKey(ChapterData.chapterName))
            {
                NowIndex = levels.IndexOf(dict[ChapterData.chapterName]);
            }
            else
            {
                dict.Add(ChapterData.chapterName, levels[NowIndex]);
            }

            LastSelectedLevels = dict;

            UpdateIndex(NowIndex, true);
            ChangeMusicPreview(false);

            _hasScrolled = true;
        }

        public void UpdateIndex(int index, bool canUpdateHarder)
        {
            var levelData = LevelData[index];

            illustration.sprite = levelData.UsingIllustration;

            if (canUpdateHarder)
            {
                LevelSelectHarder.Instance.ChangeHarder(LevelSelectHarder.CurrentHarder, false);
            }
            
            for (var i = 0; i < LevelShows.Length; i++)
            {
                var data = LevelData[i];
                LevelShows[i].Init(data, LevelSelectHarder.CurrentHarder);
            }

            topLevelShow.Init(levelData, LevelSelectHarder.CurrentHarder);

            var dict = LastSelectedLevels;
            dict[ChapterData.chapterName] = LevelData[NowIndex].identifier;
            LastSelectedLevels = dict;
        }

        private IEnumerator LoadPreviewClips()
        {
            var index = NowIndex;
            for (var i = index; i < _previewClips.Length; i++)
            {
                var clip = Resources.LoadAsync<AudioClip>(
                    $"Levels/{LevelData[i].PathFather}/{LevelData[i].identifier}/Music");
                yield return new WaitUntil(() => clip.isDone);
                _previewClips[i] = clip.asset as AudioClip;
            }

            if (index <= 0) yield break;
            for (var i = index - 1; i >= 0; i--)
            {
                var clip = Resources.LoadAsync<AudioClip>(
                    $"Levels/{LevelData[i].PathFather}/{LevelData[i].identifier}/Music");
                yield return new WaitUntil(() => clip.isDone);
                _previewClips[i] = clip.asset as AudioClip;
            }
        }

        private void ChangeMusicPreview(bool playSound)
        {
            if (_prevCoroutine != null) StopCoroutine(_prevCoroutine);

            musicSource.Pause();

            //if (playSound) UISoundManager.Play(); //TODO

            _prevCoroutine = StartCoroutine(CO_UpdateMusicVolume());
        }

        private IEnumerator CO_UpdateMusicVolume()
        {
            var levelData = LevelData[NowIndex];

            yield return new WaitUntil(() => _previewClips[NowIndex] != null);

            musicSource.clip = _previewClips[NowIndex]; //levelData.musicClip;

            const float appearTime = 1.75f; // 淡入淡出时间
            const float delayTime = 0.25f; // 停止缓冲时间

            var startTime = levelData.previewTime.x;
            var endTime = levelData.previewTime.y;

            if (startTime <= 0f)
            {
                print("音频开始预览时间有毛病, 已自动调节");
                startTime = musicSource.clip.length / 2f - appearTime - 5f;
            }

            if (endTime - startTime < appearTime * 2f)
            {
                print("音频结束预览时间有毛病, 已自动调节");
                endTime = startTime + appearTime * 2f + 5f;
            }

            var length = endTime - startTime;

            yield return new WaitForSecondsRealtime(delayTime);

            PlayMusicPreview(startTime);

            while (Instance)
            {
                var duration = Time.time - _prevStartTime;

                if (duration <= length)
                {
                    if (duration < appearTime)
                    {
                        musicSource.volume = musicVolumeCurve.Evaluate(duration / appearTime);
                    }
                    else if (duration > length - appearTime)
                    {
                        musicSource.volume = musicVolumeCurve.Evaluate((length - duration) / appearTime);
                    }
                    else
                    {
                        musicSource.volume = 1f;
                    }
                }
                else
                {
                    PlayMusicPreview(startTime);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void PlayMusicPreview(float startTime)
        {
            _prevStartTime = Time.time;
            musicSource.Pause();
            musicSource.time = startTime;
            musicSource.Play();
        }

        private void Update()
        {
            UpdateScrollInput();
            UpdateShowMovement();
        }

        private void UpdateScrollInput()
        {
            if (Input.touchCount > 0 && _canScroll)
            {
                var scroll = Input.touches.Max(x => x.deltaPosition.y);

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
                var progress =
                    bounceCurve.Evaluate(
                        Mathf.Clamp01((Time.time - (float) _bounceStartTime) / (float) _bounceDuration));
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
            for (var i = 0; i < LevelShows.Length; i++)
            {
                var pos = LevelShows[i].transform.localPosition;
                var delta = ScrollIndex - i;
                var deltaAbs = Mathf.Abs(delta);
                var deltaAbsInt = NowIndex - i;
                if (deltaAbsInt < 0) deltaAbsInt *= -1;

                if (deltaAbsInt > 8)
                {
                    if (LevelShows[i].gameObject.activeSelf)
                    {
                        LevelShows[i].gameObject.SetActive(false);
                    }

                    continue;
                }

                if (!LevelShows[i].gameObject.activeSelf)
                {
                    LevelShows[i].gameObject.SetActive(true);
                }

                var targetPosY = Mathf.Approximately(delta, 0f) ? 0f : delta * 125f + 25f * delta / deltaAbs;
                LevelShows[i].transform.localPosition = new Vector3(0f, targetPosY, pos.z);
            }
        }

        private void StartGameplay()
        {
            var levelData = LevelData[NowIndex];
    
            if (!levelData.levelHarder.HasFlag(LevelSelectHarder.CurrentHarder)) return;
        
            var path = $"Levels/{levelData.PathFather}/{levelData.identifier}/";
    
            var chartAsset = Resources.Load<TextAsset>(path + $"Chart_{LevelSelectHarder.CurrentHarder.Abbr()}");
            if (chartAsset == null) return;
            
            _canScroll = false;
    
            /*levelData.UsingMusicClip = _previewClips[NowIndex];
            levelData.UsingLevelHarder = LevelSelectHarder.CurrentHarder;
            levelData.UsingChart = ChartUtility.ReadChart(chartAsset.text);
    
            LevelManager.LevelData = levelData;
    
            StopCoroutine(_prevCoroutine);
            musicSource.DoVolume(0f, 0.25f);*/
    
            TransitionManager.DoScene("Scenes/GameScene", TransitionType.Gameplay);
        }
    }
}
