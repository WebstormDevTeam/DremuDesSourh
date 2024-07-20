using Data;
using GameUI.Global;
using GameUI.Global.Transition;
using GameUI.LevelSelect;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI.ChapterSelect
{
    [DisallowMultipleComponent]
    public class ChapterShow : MonoBehaviour
    {
        [SerializeField] private UIOutline[] outlines;
        [SerializeField] private Text chapterName, subtitle;
        [SerializeField] private Button entryButton;
        [SerializeField] private Image illustration;

        public UIOutline[] Outlines => outlines;

        public void Init(int index, ChapterData data)
        {
            chapterName.text = data.chapterName;
            subtitle.text = data.subTitle;

            illustration.sprite = Resources.Load<Sprite>($"Chapters/{data.identifier}/Illustration");
            
            entryButton.onClick.RemoveAllListeners();
            entryButton.onClick.AddListener(() =>
            {
                var chapter = ChapterSelectManager.ChapterData[index];
                LevelSelectManager.SetSceneToGo("Scenes/ChapterSelectScene");
                LevelSelectManager.ChapterData = chapter;
                TransitionManager.DoScene("LevelSelectScene", TransitionType.DefaultWhite);
            });
        }
    }
}
