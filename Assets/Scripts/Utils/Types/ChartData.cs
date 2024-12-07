using System.Collections.Generic;
using System.Numerics;


namespace Utils.Types
{
    public class ChartData
    {
        public string Version;
        public string Name;
        public string Artist;
        public string Hard;
        public int DefaultBPM;
        public Data Data;
    }

    public class Data
    {
        public List<BpmListItem> BPMList;
        public List<JudgementLine> JudgementLineList;
    }

    public class BpmListItem
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
        public List<Vector2> Points;
        public List<List<Vector2>> Nodes;
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