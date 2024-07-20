using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class LevelData
    {
        [Title("单项数据")]
        
        [LabelText("关卡ID (资源文件夹名)")] public string identifier;
        [LabelText("关卡名")] public string musicName;
        [LabelText("曲师名")] public string composerName;
        [LabelText("画师名")] public string illustratorName;
        [LabelText("音频预览时间 (开始和结束/s)")] public Vector2 previewTime;
        
        [Title("多项数据")]
        
        [LabelText("关卡拥有的难度")] public LevelHarder levelHarder;
        [LabelText("谱师列表")] public string[] charterName;
        [LabelText("定数列表 (没有完成填0.修改次数) (?填-1)")] public float[] levelDifficulty;

        [NonSerialized] public AudioClip UsingMusicClip;
        [NonSerialized] public Sprite UsingIllustration;
        [NonSerialized] public LevelHarder UsingLevelHarder;
        //[NonSerialized] public ChartData UsingChart;
        [NonSerialized] public string PathFather;
    }

    [Flags]
    public enum LevelHarder
    {
        Simple = 1 << 0, 
        Common = 1 << 1,
        Complex = 1 << 2, 
        Overload = 1 << 3,
        Extra = 1 << 4,
        Unknown = 1 << 5
    }

    public static class LevelDataHelper
    {
        public static int Index(this LevelHarder harder)
        {
            return harder switch
            {
                LevelHarder.Simple => 0,
                LevelHarder.Common => 1,
                LevelHarder.Complex => 2,
                LevelHarder.Overload => 3,
                LevelHarder.Extra => 4,
                LevelHarder.Unknown => 5,
                0 => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(harder), harder, "Invalid Level Harder")
            };
        }

        public static string Abbr(this LevelHarder harder)
        {
            return harder switch
            {
                LevelHarder.Simple => "SP",
                LevelHarder.Common => "CM",
                LevelHarder.Complex => "CL",
                LevelHarder.Overload => "OL",
                LevelHarder.Extra => "EX",
                LevelHarder.Unknown => "UN",
                0 => "-",
                _ => throw new ArgumentOutOfRangeException(nameof(harder), harder, "Invalid Level Harder")
            };
        }
        
        public static LevelHarder Max(this LevelHarder harder)
        {
            if (harder.HasFlag(LevelHarder.Overload)) return LevelHarder.Overload;
            if (harder.HasFlag(LevelHarder.Complex)) return LevelHarder.Complex;
            if (harder.HasFlag(LevelHarder.Common)) return LevelHarder.Common;
            if (harder.HasFlag(LevelHarder.Simple)) return LevelHarder.Simple;
            return LevelHarder.Simple;
        }
        
        public static LevelHarder Min(this LevelHarder harder)
        {
            if (harder.HasFlag(LevelHarder.Simple)) return LevelHarder.Simple;
            if (harder.HasFlag(LevelHarder.Common)) return LevelHarder.Common;
            if (harder.HasFlag(LevelHarder.Complex)) return LevelHarder.Complex;
            if (harder.HasFlag(LevelHarder.Overload)) return LevelHarder.Overload;
            return LevelHarder.Simple;
        }

        public static Color SignColor(this LevelHarder harder)
        {
            return harder switch
            {
                _ => throw new ArgumentOutOfRangeException(nameof(harder), harder, "暂时不可用")
                //TODO：加上难度标识色
            };
        }
    
        public static string GetDifficultyName(float diff)
        {
            return diff switch
            {
                >= 0 and < 1 => GetUnfinishedFlag(diff),
                -1 => "?",
                _ => $"{diff:F0}"
            };

            static string GetUnfinishedFlag(float diff)
            {
                var mods = $"{diff}".Split('.');
                var mod = mods.Length > 1
                    ? mods[1].Replace("0", "")
                    : mods[0];
                return $"T{mod}";
            }
        }
    }
}