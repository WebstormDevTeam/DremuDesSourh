using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string jsonString = json;
            JObject chartData = JObject.Parse(jsonString);
            return chartData;
        }

        public static Root GetChartDataToRoot(string json)
        {
            string jsonString = json;
            Root chartData = JsonConvert.DeserializeObject<Root>(jsonString);
            return chartData;
        }
        
        
    }

    public class Root
    {
        public string Version { get; set; }
        public string Name{ get; set; }
        public string Artist { get; set; }
        public string Hard{ get; set; }
        public int DefaultBPM { get; set; }
        public ChartData ChartData { get; set; }
    }

    public class ChartData
    {
        public List<BPMListItem> BPMList { get; set; }
        public List<JudgementLine> JudgementLineList { get; set; }
    }

    public class BPMListItem
    {
        public int Time { get; set; }
        public int Value { get; set; }
        public int Tension { get; set; }
        public string CurveType { get; set; }
    }

    public class JudgementLine
    {
       public List<Dremu.Gameplay.Object.ChartData.JudgmentLineData.Curve> CurveGroup{ get; set; } 
       public EnvelopeLine EnvelopeLine { get; set; }
       public List<Note> Note { get; set; }
       public List<GuideLine> GuideLine { get; set; }
    }

    public class Curve
    {
        public List<List<int>> Points { get; set; }
        public List<List<List<int>>> Nodes { get; set; }
    }

    public class EnvelopeLine
    {
        public List<Controller> Controllers { get; set; }
    }

    public class Controller
    {
        public int Time { get; set; }
        public int Value { get; set; }
        public int Tension { get; set; }
        public string CurveType { get; set; }
    }

    public class Note
    {
        public int Time { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
    }

    public class GuideLine
    {
        public float Position { get; set; }
        public int ArrivalTime { get; set; }
        public List<Node> Nodes { get; set; }
    }

    public class Node
    {
        public float To { get; set; }
        public int Time { get; set; }
        public string CurveType { get; set; }
    }
}