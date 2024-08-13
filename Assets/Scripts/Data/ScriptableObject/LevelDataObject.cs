using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Data.ScriptableObject
{
    [CreateAssetMenu(menuName = "Dremu SO/Level Data", fileName = "New Level Data")]
    public class LevelDataObject : UnityEngine.ScriptableObject
    {
        [SerializeField, LabelText("关卡数据")] private LevelData levelData;

        [SerializeField, LabelText("资源文件夹路径父节点名 (必填)")]
        private string pathFather;

        public LevelData CurrentData
        {
            get
            {
                levelData.PathFather = pathFather;
                return levelData;
            }
        }

        public string PathFather => pathFather;

#if UNITY_EDITOR

        [ContextMenu("Bind Data From Name")]
        private void BindDataFromName()
        {
            var str = name ?? "";
            var lines = str.Split('_');
            levelData.identifier = str;

            if (!lines.IsNullOrEmpty() && lines.Length > 1)
            {
                levelData.musicName = lines[0];
                levelData.composerName = lines[1];
            }
        }

#endif
    }
}
