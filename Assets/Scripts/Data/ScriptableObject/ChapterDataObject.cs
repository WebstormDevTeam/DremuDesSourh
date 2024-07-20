using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.ScriptableObject
{
    [CreateAssetMenu(menuName = "Simple SO/Chapter Data", fileName = "New Chapter Data")]
    public class ChapterDataObject : UnityEngine.ScriptableObject
    {
        [SerializeField, LabelText("章节ID (资源文件夹名)")] private string identifier;
        [SerializeField, LabelText("章节名 (如 Chapter I)")] private string chapterName;
        [SerializeField, LabelText("章节副标题 (如 嘉然我真的好喜欢你啊😅)")] private string chapterSubTitle;
        [SerializeField, LabelText("关卡文件们")] private LevelDataObject[] levelDataObjects;
        public ChapterData CurrentData => new(identifier, chapterName, chapterSubTitle, levelDataObjects);
    }
}