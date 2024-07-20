using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Tool
{
    // 具体的连击分还没定
    public static class ScoreManager
    {
        static int scoreForACC = 0;
        static int totalNotes = 0;
        static int comboNow = 0;
        static int comboMax = 0;

        static int perfect = 0;
        static int good = 0;
        static int bad = 0;
        static int miss = 0;

        public static void Reset() { 
            scoreForACC = totalNotes = comboNow = comboMax = perfect = good = bad = miss = 0; 
        }
        public static void SetTotalNotes(int count) { totalNotes = count; }
        public static int Score
        {
            get
            { 
                float score = 0;
                comboMax = Mathf.Max(comboNow, comboMax);
                for (int i = 1; i <= comboMax; i++)
                    score += ComboScore(i, totalNotes);
                return Mathf.RoundToInt(score) + scoreForACC;
            }
        }

        // 连击分计算公式
        private static float ComboScore(int x, int max)
        {
            if (x < max / 2)
                return -(20 / max) * x;
            else
                return 5.56f;
        }
        public static float ACC { get { return scoreForACC / ( totalNotes * 100 ); } }

        public static void AddGood() {comboNow++; scoreForACC += 50; good++; }
        public static void AddPerfect() {comboNow++; scoreForACC += 100; perfect++; }
        public static void AddBad() { comboMax = Mathf.Max(comboNow, comboMax); comboNow = 0; bad++; }
        public static void AddMiss() { comboMax = Mathf.Max(comboNow, comboMax); comboNow = 0; miss++; }
        
    }
}