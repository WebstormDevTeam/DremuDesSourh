using Data;
using GameUI.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.LevelSelect
{
    [DisallowMultipleComponent]
    public class LevelShow : MonoBehaviour
    {
        [SerializeField] private UIOutline outline;

        [SerializeField] private bool isTop;

        [SerializeField] private Text musicName;
        
        [SerializeField, HideIf(nameof(isTop))] private Text difficulty;
        
        [SerializeField, ShowIf(nameof(isTop))] private Text composerName, result, score, accuracy;

        public UIOutline Outline => outline;

        public void Init(LevelData data, LevelHarder harder)
        {
            musicName.text = data.musicName;
            difficulty.text = data.levelHarder.HasFlag(harder)
                ? $"{data.levelDifficulty[harder.Index()]}"
                : "-";

            if (isTop)
            {
                composerName.text = data.composerName;
                //TODO: Result Accuracy Score
            }
        }
    }
}
