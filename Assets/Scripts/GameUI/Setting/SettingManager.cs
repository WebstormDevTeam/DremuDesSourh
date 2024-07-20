using GameUI.Global;
using GameUI.Global.Transition;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utils.Singleton;

namespace GameUI.Setting
{
    [DisallowMultipleComponent]
    
   
    
    public class SettingManager : MonoBehaviourSingleton<SettingManager>
    {
        [Title("组件们")]
        [SerializeField] private Button back;
        protected override void OnAwake()
        {
            back.onClick.AddListener(() =>
            {
                TransitionManager.DoScene("Scenes/MainUIScene",TransitionType.DefaultWhite);
            });
        }
    }
}