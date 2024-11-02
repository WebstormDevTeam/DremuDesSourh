using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Utils.Helper.ChartHelper
{
    public class ChartAnalyser
    {
        /// <summary>
        /// 获取谱面数据
        /// </summary>
        /// <param name="json">传入一个字符串，字符串是JSON格式的谱面文件</param>
        /// <returns>谱面数据，JObject对象格式</returns>
        public static JObject GetChartDataToJObject(string json)
        {
            string jsonString = File.ReadAllText(json);
            JObject chartData = JObject.Parse(jsonString);
            return chartData;
        }

        public static Root GetChartDataToRoot(string json)
        {
            string jsonString = File.ReadAllText(json);
            Debug.Log(jsonString);
            Root chartData = JsonConvert.DeserializeObject<Root>(jsonString);
            return chartData;
        }


    }

    public class Root
    {
        public string Version;
        public string Name;
        public string Artist;
        public string Hard;
        public int DefaultBPM;
        public ChartData ChartData;
    }

    public class ChartData
    {
        public List<BPMListItem> BPMList;
        public List<JudgementLine> JudgementLineList;
    }

    public class BPMListItem
    {
        public int Time;
        public float Value;
        public int Tension;
        public string CurveType;
    }

    public class JudgementLine
    {
        public List<Curve> CurveGroup;
        public EnvelopeLine EnvelopeLine;
        public List<Note> Note;
        public List<GuideLine> GuideLine;
    }

    public class Curve
    {
        public List<List<float>> Points;
        public List<List<List<float>>> Nodes;
    }

    public class EnvelopeLine
    {
        public List<Controller> Controllers;
    }

    public class Controller
    {
        public int Time;
        public float Value;
        public int Tension;
        public string CurveType;
    }

    public class Note
    {
        public int Time;
        public string Type;
        public float Value;
    }

    public class GuideLine
    {
        public float Position;
        public int ArrivalTime;
        public List<Node> Nodes;
    }

    public class Node
    {
        public float To;
        public int Time;
        public string CurveType;
    }
}