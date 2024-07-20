using Data;
using GameUI.Global.Sound;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Utils.Singleton;

namespace GameUI.LevelSelect
{
    public class LevelSelectHarder : MonoBehaviourSingleton<LevelSelectHarder>
    {
        [SerializeField] private Button target;
        [SerializeField] private Text harderText, difficultyText;

        private static LevelHarder _levelHarder = LevelHarder.Simple;
        public static LevelHarder CurrentHarder
        {
            get => _levelHarder;
            set
            {
                if (_levelHarder > 0)
                {
                    _levelHarder = value;
                }
                Instance.UpdateShow();
            }
        }
        
        protected override void OnAwake()
        {
            target.onClick.AddListener(AddHarder);
        }

        private void Start()
        {
            var lastHarder = PlayerPrefs.GetInt("LevelSelect_LastHarder", 1);
            _levelHarder = (LevelHarder) lastHarder;
            ChangeHarder(_levelHarder, true);
        }

        private void AddHarder()
        {
            if (LevelSelectManager.Instance.ScrollIndex % 1 >= 0.01f) return;
            
            //UISoundManager.Play("Selector_Change_0");
            
            var nowHarder = CurrentHarder;
            var harder = LevelSelectManager.LevelData[LevelSelectManager.Instance.NowIndex].levelHarder;
            
            while (nowHarder <= LevelHarder.Overload)
            {
                nowHarder = (LevelHarder) ((int) nowHarder << 1);
                if (harder.HasFlag(nowHarder)) break;
            }
            
            if (nowHarder > harder.Max())
            {
                nowHarder = harder.Min();
            }
        
            ChangeHarder(nowHarder, true);
        }

        public void ChangeHarder(LevelHarder levelHarder, bool canUpdateIndex)
        {
            var harder = LevelSelectManager.LevelData[LevelSelectManager.Instance.NowIndex].levelHarder;
            
            if (levelHarder > harder.Max() || !harder.HasFlag(levelHarder))
            {
                levelHarder = harder.Max();
            }

            CurrentHarder = levelHarder;
            PlayerPrefs.SetInt("LevelSelect_LastHarder", (int) CurrentHarder);
            
            if (canUpdateIndex)
            {
                LevelSelectManager.Instance.UpdateIndex(LevelSelectManager.Instance.NowIndex, false);
            }
            
            var levelData = LevelSelectManager.LevelData[LevelSelectManager.Instance.NowIndex];
            harderText.text = $"{CurrentHarder}";
            difficultyText.text = $"{levelData.levelDifficulty[CurrentHarder.Index()]}";
        }

        private void UpdateShow()
        {
            if (LevelSelectManager.Instance.LevelShows.IsNullOrEmpty()) return;
            
            for (var i = 0; i < LevelSelectManager.Instance.LevelShows.Length; i++)
            {
                var level = LevelSelectManager.Instance.LevelShows[i];
                var data = LevelSelectManager.LevelData[i];
                level.Init(data, CurrentHarder);
            }
        }
    }
}
