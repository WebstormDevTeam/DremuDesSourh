using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Dremu.Gameplay.Manager;
using Dremu.Gameplay.Tool;
using Dremu.Gameplay.Object;
using Utils.Helper.ChartHelper;
using Curve = Dremu.Gameplay.Tool.Curve;
using EnvelopeLine = Dremu.Gameplay.Tool.EnvelopeLine;
using GuideLine = Dremu.Gameplay.Object.GuideLine;
using JudgementLine = Dremu.Gameplay.Object.JudgementLine;
using Tap = Dremu.Gameplay.Object.Tap;
using Slide = Dremu.Gameplay.Object.Slide;

namespace Utils.Helper.ChartConvertHelper
{
    public class Chart
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
    
    public class JudgementLine
    {
        public List<Curve> Curves;
        public EnvelopeLine CurveController;
        public EnvelopeLine Speed;
        public EnvelopeLine Alpha;
        public Notes Notes;
        public List<GuideLine> GuideLines;
    }
    
    public class GuideLine
    {
        public float Position;
        public int ArrivalTime;
        public List<Node> Nodes;
    }
    
    public class ChartConvertHelper
    {
        
        public static Chart ConvertChartFromRoot(Root root)
        {
            if (root == null) throw new NullReferenceException();
            return new Chart()
            {
                Version = root.Version,
                Name = root.Name,
                Artist = root.Artist,
                Hard = root.Hard,
                DefaultBPM = root.DefaultBPM,
                ChartData = new ChartData()
                {
                    BPMList = root.ChartData.BPMList,
                    JudgementLineList = ConvertJudgementLines(root.ChartData.JudgementLineList)
                }
            };
        }
        
        

        public static List<JudgementLine> ConvertJudgementLines(List<Utils.Helper.ChartHelper.JudgementLine> chartJudgementLines)
        {
            var t = new List<JudgementLine>();
            foreach (var jLine in chartJudgementLines)
            {
                t.Add(new JudgementLine()
                {
                    Curves = ConvertCurves(jLine.CurveGroup),
                    CurveController = ConvertEnvelopeLines(jLine.EnvelopeLine),
                    Speed = ConvertEnvelopeLines(jLine.EnvelopeSpeed),
                    Alpha = ConvertEnvelopeLines(jLine.EnvelopeAlpha),
                    Notes = jLine.Notes,
                    GuideLines = ConvertGuideLines(jLine.GuideLines)
                });
            }
            return t;
        }
        
        public static List<Curve> ConvertCurves(List<Utils.Helper.ChartHelper.Curve> chartCurves)
        {
            var t = new List<Curve>();
            foreach (var chartCurve in chartCurves)
            {
                t.Add(new Curve(chartCurve.Points, chartCurve.Nodes));
            }
            return t;
        }
        
        public static ControllNode ConvertController(Utils.Helper.ChartHelper.Controller chartController)
        {
            return new ControllNode(chartController.Time, chartController.Value, chartController.Tension, EaseTypeManager.StringToCurveType(chartController.CurveType) );
        }
        
        public static EnvelopeLine ConvertEnvelopeLines(Utils.Helper.ChartHelper.EnvelopeLine chartEnvelopeLines)
        {
            var t = new EnvelopeLine(new List<ControllNode> { });
            foreach (var chartController in chartEnvelopeLines.Controllers)
            {
                t.Controllers.Add(ConvertController(chartController));
            }
            return t;
        }
        
        public static List<GuideLine> ConvertGuideLines(List<Utils.Helper.ChartHelper.GuideLine> chartGuideLines)
        {
            var t = new List<GuideLine> { };
            foreach (var chartGuideLine in chartGuideLines)
            {
                t.Add(new GuideLine()
                {
                    Position = chartGuideLine.Position,
                    ArrivalTime = chartGuideLine.ArrivalTime,
                    Nodes = chartGuideLine.Nodes
                });
            }
            return t;
        }

        public static List<Dremu.Gameplay.Object.GuideLine.GuideNode> ConvertNodes(List<Utils.Helper.ChartHelper.Node> chartNodes)
        {
            List<Dremu.Gameplay.Object.GuideLine.GuideNode> t = new List<Dremu.Gameplay.Object.GuideLine.GuideNode>();
            foreach (var chartNode in chartNodes)
            {
                t.Add(new Dremu.Gameplay.Object.GuideLine.GuideNode(chartNode.To, chartNode.Time, EaseTypeManager.EaseType.LINEAR));
                //TODO:缓动类型待修改
            }
            return t;
        }
        
        
    }
}