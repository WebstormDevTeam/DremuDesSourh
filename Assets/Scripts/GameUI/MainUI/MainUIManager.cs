using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Data.ScriptableObject;
using DG.Tweening;
using GameUI.ChapterSelect;
using GameUI.Global.Transition;
using GameUI.Setting;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils.Helper;
using Utils.Singleton;

namespace GameUI.MainUI
{
    [DisallowMultipleComponent]
    public class MainUIManager : MonoBehaviourSingleton<MainUIManager>
    {
        [Title("组件们")]
        [SerializeField, LabelText("白屏渐出")] private Image whiteMask;
        [SerializeField, LabelText("欢迎界面")] private CanvasGroup welcomePage;
        [SerializeField, LabelText("主界面")] private CanvasGroup mainPage;
        [SerializeField, LabelText("触发按钮")] private Button entryButton;
        [SerializeField, LabelText("标题")] private Text titleText;
        [SerializeField] private Button storyModeButton, freeModeButton, settingsButton;

        [Title("数值们")]
        [SerializeField, LabelText("标题移动曲线")] private AnimationCurve titleMoveCurve;
        [SerializeField, LabelText("标题放缩曲线")] private AnimationCurve titleScaleCurve;

        [Title("文件们")]
        [SerializeField, LabelText("章节文件们")] private ChapterDataObject[] chapterDataObjects;

        private static bool _isWelcomed = false;

        protected override async void OnAwake()
        {
            //游戏设置横屏锁定，只能在横向
            Application.targetFrameRate = 120;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.sleepTimeout = SleepTimeout.NeverSleep; // TODO：之后写进Gameplay里面

            ChapterSelectManager.ChapterData = chapterDataObjects.Select(x => x.CurrentData).ToArray();

            //跳转LevelSelect的按钮
            storyModeButton.onClick.AddListener(() =>
            {
                /*ChapterSelectManager.SetSceneToGo("Scenes/MainUIScene");
                TransitionManager.DoScene("Scenes/LevelSelectScene", TransitionType.DefaultWhite);*/
            });
            //跳转Free模式的按钮
            freeModeButton.onClick.AddListener(() =>
            {
                ChapterSelectManager.SetSceneToGo("Scenes/MainUIScene");
                TransitionManager.DoScene("Scenes/ChapterSelectScene", TransitionType.DefaultWhite);
            });
            //跳转设置界面
            settingsButton.onClick.AddListener(() =>
            {
                SettingManager.SetSceneToGo("Scenes/MainUIScene");
                TransitionManager.DoScene("Scenes/SettingScene", TransitionType.DefaultWhite);
            });

            var titlePos = new Vector2(-900f, (float)Screen.height / Screen.width / (9f / 16f) * 540f - 90f);
            var titleScl = new Vector3(0.45f, 0.45f, 1f);

            if (_isWelcomed)
            {
                welcomePage.gameObject.SetActive(false);
                mainPage.gameObject.SetActive(true);
                mainPage.alpha = 1f;
                titleText.rectTransform.anchoredPosition = titlePos;
                titleText.rectTransform.localScale = titleScl;
            }
            else
            {
                whiteMask.gameObject.SetActive(true);
                whiteMask.color = Color.white;

                var config = AudioSettings.GetConfiguration();
                var dsp = 256; // TODO：改DSP初始值
                config.dspBufferSize = dsp;
                AudioSettings.Reset(config);

                await Task.Delay(500);

                welcomePage.gameObject.SetActive(true);
                mainPage.gameObject.SetActive(false);

                whiteMask.DOColor(Color.white.SetAlpha(0f), 0.5f).SetEase(DG.Tweening.Ease.OutSine).OnComplete(() =>
                {
                    whiteMask.gameObject.SetActive(false);

                    entryButton.onClick.AddListener(() =>
                    {
                        if (!_isWelcomed)
                        {
                            _isWelcomed = true;

                            titleText.rectTransform.DOAnchorPos(titlePos, 0.5f).SetEase(titleMoveCurve);
                            titleText.rectTransform.DOScale(titleScl, 0.5f).SetEase(titleScaleCurve);

                            welcomePage.DoFade(0f, 0.5f).SetEase(DG.Tweening.Ease.OutSine).OnComplete(() => welcomePage.gameObject.SetActive(false));

                            StartCoroutine(AniFadeInAndMoveUp(0.2f));
                        }
                    });
                });
            }
        }

        private void Update()
        {

#if UNITY_EDITOR
            if (Mathf.Approximately(titleText.rectTransform.anchoredPosition.x, -900f))
                titleText.rectTransform.anchoredPosition = new Vector2(-900f, (float)Screen.height / Screen.width / (9f / 16f) * 540f - 90f);
#endif

        }

        /// <summary>
        /// 淡入并上移动画
        /// </summary>
        /// <param name="waitTime">延时间隔秒数</param>
        /// <returns></returns>
        private IEnumerator AniFadeInAndMoveUp(float waitTime)
        {
            foreach (Transform child in mainPage.transform)
                child.GetComponent<Image>().color = Color.clear;

            mainPage.alpha = 0f;
            mainPage.gameObject.SetActive(true);
            mainPage.DoFade(1f, 0.5f).SetEase(DG.Tweening.Ease.OutSine);

            foreach (Transform child in mainPage.transform)
            {
                Debug.Log(child.name);

                child.GetComponent<Image>().DOColor(Color.white, 0.5f).SetEase(DG.Tweening.Ease.OutSine);
                child.DOMoveY(-15f, 1f).From(true).SetEase(DG.Tweening.Ease.OutSine);

                yield return new WaitForSeconds(waitTime);
            }  
        }
    }
}
