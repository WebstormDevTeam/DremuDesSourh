using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils.Helper;
using Utils.Singleton;

namespace GameUI.Global.Transition
{
    [DisallowMultipleComponent]
    public class TransitionManager : MonoBehaviourSingleton<TransitionManager>
    {
        [Header("Default")]
        [SerializeField] private CanvasGroup defaultGroup;
        [SerializeField] private Image defaultImage;

        [Header("Gameplay")]
        [SerializeField] private CanvasGroup gameplayGroup;

        private string _sceneName;

        protected override void OnAwake()
        {
            defaultGroup.alpha = 0f;
            gameplayGroup.alpha = 0f;
            defaultGroup.SetActiveAndInteractable(false);
            gameplayGroup.SetActiveAndInteractable(false);
        }

        public static void DoScene(string sceneName, TransitionType type)
        {
            if (!Instance)
            {
                SceneManager.LoadSceneAsync(sceneName);
                return;
            }

            Instance._sceneName = sceneName;
            
            var _ = type switch
            {
                TransitionType.DefaultBlack => Instance.Default(Color.black),
                TransitionType.DefaultWhite => Instance.Default(Color.white),
                TransitionType.Gameplay => Instance.Gameplay(),
                _ => Instance.Default(Color.black)
            };
        }

        private async Task Default(Color color, float sceneDuration = 0.75f)
        {
            defaultGroup.SetActiveAndInteractable(true);
            
            defaultImage.color = color;
            defaultGroup.DoFade(1f, 0.5f).SetEase(Ease.OutSine);
            
            await Task.Delay(500);
            await GotoScene(sceneDuration);

            defaultGroup.DoFade(0f, 0.5f).SetEase(Ease.OutSine);
            
            await Task.Delay(500);
            
            defaultGroup.SetActiveAndInteractable(false);
        }

        private async Task Gameplay()
        {
            //TODO
        }

        private async Task GotoScene(float duration = 0.5f)
        { 
            var operation = SceneManager.LoadSceneAsync(_sceneName);
            operation.allowSceneActivation = false;
        
            await Task.Delay(TimeSpan.FromSeconds(duration));

            operation.allowSceneActivation = true;

            await operation.ToTask();
        }
    }
    
    public enum TransitionType
    {
        DefaultBlack = 0, DefaultWhite = 1, Gameplay = 3
    }

    public static class TransitionHelper
    {
        public static void SetActiveAndInteractable(this CanvasGroup group, bool flag)
        {
            SetInteractable(group, flag);
            SetActive(group, flag);
        }

        public static void SetActive(this CanvasGroup group, bool flag)
        {
            group.gameObject.SetActive(flag);
        }

        public static void SetInteractable(this CanvasGroup group, bool flag)
        {
            group.interactable = flag;
            group.blocksRaycasts = flag;
        }
    }
}
