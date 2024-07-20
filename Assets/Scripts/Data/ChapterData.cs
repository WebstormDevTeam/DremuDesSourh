using System;
using System.Collections.Generic;
using System.Linq;
using Data.ScriptableObject;

namespace Data
{
    [Serializable]
    public class ChapterData
    {
        public string identifier;
        public string chapterName;
        public string subTitle;
        public LevelData[] levelData;

        public ChapterData(string id, string name, string newSubTitle, IEnumerable<LevelDataObject> levelDataObjects)
        {
            identifier = id;
            chapterName = name;
            subTitle = newSubTitle;
            levelData = levelDataObjects.Select(x =>
            {
                var data = x.CurrentData;
                data.PathFather = x.PathFather;
                return data;
            }).ToArray();
        }
    }
}