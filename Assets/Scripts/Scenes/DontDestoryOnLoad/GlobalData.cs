/*
 * Copyright (C) 2024 BlophyNovaEdit
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * 注意，在没有修改的代码我们同样尊重MPL2.0的条款。只不过我们没有特别注明。
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Data.ChartData;
using Data.ChartEdit;
using Data.Enumerate;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using UtilityCode.Singleton;
namespace Scenes.DontDestoryOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        // 当前难度设置
        public string currentHard;
        // ChartData和ChartEditData分别用于存储和编辑图表数据
        public Data.ChartData.ChartData chartData;
        public Data.ChartEdit.ChartData chartEditData;
        // 音频剪辑
        public AudioClip clip;
        // 当前CP和CPH的精灵（Sprite），CP和CPH可能是游戏中的某种元素
        [FormerlySerializedAs("currentCP")]
        public Sprite currentCp;
        [FormerlySerializedAs("currentCPH")]
        public Sprite currentCph;
        // 是否自动播放
        [SerializeField]public bool isAutoplay = true;
        // 偏移量
        [SerializeField]public float offset;
        // 屏幕宽度和高度的属性，使用Camera.main的像素尺寸
        public int ScreenWidth => Camera.main.pixelWidth;
        public int ScreenHeight => Camera.main.pixelHeight;
        
        // 其他游戏相关数据和预设

        [SerializeField]public Alert alert;

        public List<LabelWindowContent> labelWindowContents = new();


        [SerializeField]public LabelItem labelItemPrefab;
        [SerializeField]public LabelWindow labelWindowPrefab;

        public EventEditItem eventEditItem;

        public TapEdit tapEditPrefab;
        public DragEdit dragEditPrefab;
        public HoldEdit holdEditPrefab;
        public FlickEdit flickEditPrefab;
        public PointEdit pointEditPrefab;
        public FullFlickEdit fullFlickEditPrefab;

        public List<EaseData> easeData;
        // 用于刷新游戏内所有需要刷新的接口的静态方法
        public static void Refresh()
        {
            //调用AssemblySystem的静态方法Exe，执行所有实现了IRefresh接口的类的Refresh方法
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<IRefresh>(), (interfaceMethod) => interfaceMethod.Refresh());
        }
        // 添加NoteEdit到ChartData的方法
        public void AddNoteEdit2ChartData(Data.ChartEdit.Note noteEdit,int boxID,int lineID)
        {
            // 通过二分查找找到插入位置，确保按时间顺序排列
            int index_noteEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[lineID].onlineNotes, m => m.hitBeats.ThisStartBPM < noteEdit.hitBeats.ThisStartBPM, false);
            // 创建一个新的Note并插入到ChartEditData和ChartData中
            Data.ChartData.Note note = new(noteEdit);
            chartEditData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteEdits, noteEdit);
            chartData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteEdits,note);
        }
        // 生命周期方法，确保此游戏对象不会在加载新场景时被销毁
        protected override void OnAwake()
        {
            DontDestroyOnLoad(gameObject);
        }
        // 读取资源
        public IEnumerator ReadResource()
        {
            UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.streamingAssetsPath}/-1/Music/Music.mp3", AudioType.MPEG);
            yield return unityWebRequest.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/-1/Illustration/CPH.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCph = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/-1/Illustration/CP.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cp = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCp = Sprite.Create(cp, new Rect(0, 0, cp.width, cp.height), new Vector2(0.5f, 0.5f));
        }
        // 在游戏开始时加载资源
        private void Start()
        {
            // 设置目标帧率为9999，即尽可能高的帧率
            Application.targetFrameRate = 9999;
            // 从流资源路径读取EaseData配置文件
            easeData = JsonConvert.DeserializeObject<List<EaseData>>(File.ReadAllText($"{Application.streamingAssetsPath}/Config/EaseData.json")); 
            // 创建新的图表数据
            CreateNewChart();
            // 将ChartEditData转换为ChartData
            chartData.boxes= ConvertChartEdit2ChartData(chartEditData.boxes);
        }
        // 创建新图表数据的方法
        public void CreateNewChart()
        {
            // 初始化ChartEditData
            chartEditData.boxes = new();



            // 初始化一个新的Box
            Data.ChartEdit.Box chartEditBox = new();
            // 初始化Box的Lines和BoxEvents
            chartEditBox.lines = new() { new(), new(), new(), new(), new() };
            chartEditBox.boxEvents = new();
            chartEditBox.boxEvents.scaleX = new();
            chartEditBox.boxEvents.scaleY = new();
            chartEditBox.boxEvents.moveX = new();
            chartEditBox.boxEvents.moveY = new();
            chartEditBox.boxEvents.centerX = new();
            chartEditBox.boxEvents.centerY = new();
            chartEditBox.boxEvents.alpha = new();
            chartEditBox.boxEvents.lineAlpha = new();
            chartEditBox.boxEvents.rotate = new();
            chartEditBox.boxEvents.scaleX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
            chartEditBox.boxEvents.scaleY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
            chartEditBox.boxEvents.moveX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.moveY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.centerX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = .5f, endValue = .5f, curve = easeData[0] });
            chartEditBox.boxEvents.centerY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = .5f, endValue = .5f, curve = easeData[0] });
            chartEditBox.boxEvents.alpha.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 1, endValue = 1, curve = easeData[0] });
            chartEditBox.boxEvents.lineAlpha.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.rotate.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            for (int i = 0; i < chartEditBox.lines.Count; i++)
            {
                chartEditBox.lines[i].offlineNotes = new();
                chartEditBox.lines[i].onlineNotes = new();
                chartEditBox.lines[i].speed = new();
                chartEditBox.lines[i].speed.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 3, endValue = 3, curve = easeData[0] });
            }
            // 将初始化的Box添加到ChartEditData中
            chartEditData.boxes.Add(chartEditBox);
        }
        // 将ChartEditData转换为ChartData的方法
        public List<Data.ChartData.Box> ConvertChartEdit2ChartData(List<Data.ChartEdit.Box> boxes)
        {
            // 转换过程，创建新的ChartData对象并填充数据
            List<Data.ChartData.Box> result = new();
            foreach (Data.ChartEdit.Box box in boxes)
            {
                Data.ChartData.Box chartDataBox = new();
                chartDataBox.lines = new() { new(), new(), new(), new(), new() };
                chartDataBox.boxEvents = new();
                chartDataBox.boxEvents.scaleX = new();
                chartDataBox.boxEvents.scaleY = new();
                chartDataBox.boxEvents.moveX = new();
                chartDataBox.boxEvents.moveY = new();
                chartDataBox.boxEvents.centerX = new();
                chartDataBox.boxEvents.centerY = new();
                chartDataBox.boxEvents.alpha = new();
                chartDataBox.boxEvents.lineAlpha = new();
                chartDataBox.boxEvents.rotate = new();
                ForeachBoxEvents(box.boxEvents.scaleX, chartDataBox.boxEvents.scaleX);
                ForeachBoxEvents(box.boxEvents.scaleY, chartDataBox.boxEvents.scaleY);
                ForeachBoxEvents(box.boxEvents.moveX, chartDataBox.boxEvents.moveX);
                ForeachBoxEvents(box.boxEvents.moveY, chartDataBox.boxEvents.moveY);
                ForeachBoxEvents(box.boxEvents.centerX, chartDataBox.boxEvents.centerX);
                ForeachBoxEvents(box.boxEvents.centerY, chartDataBox.boxEvents.centerY);
                ForeachBoxEvents(box.boxEvents.alpha, chartDataBox.boxEvents.alpha);
                ForeachBoxEvents(box.boxEvents.lineAlpha, chartDataBox.boxEvents.lineAlpha);
                ForeachBoxEvents(box.boxEvents.rotate, chartDataBox.boxEvents.rotate);
                for (int i = 0; i < chartDataBox.lines.Count; i++)
                {
                    chartDataBox.lines[i].offlineNotes = new();
                    foreach (Data.ChartEdit.Note item in box.lines[i].offlineNotes)
                    {
                        Data.ChartData.Note newChartDataNote = new(item);
                        chartDataBox.lines[i].offlineNotes.Add(newChartDataNote);
                    }
                    chartDataBox.lines[i].onlineNotes = new();
                    foreach (Data.ChartEdit.Note item in box.lines[i].onlineNotes)
                    {
                        Data.ChartData.Note newChartDataNote = new(item);
                        chartDataBox.lines[i].onlineNotes.Add(newChartDataNote);
                    }
                    List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(box.lines[i].speed);
                    chartDataBox.lines[i].speed = new();
                    ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
                    chartDataBox.lines[i].career = new() { postWrapMode=WrapMode.ClampForever,preWrapMode=WrapMode.ClampForever};
                    chartDataBox.lines[i].career.keys = GameUtility.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray();
                    chartDataBox.lines[i].far = new() { postWrapMode=WrapMode.ClampForever,preWrapMode=WrapMode.ClampForever};
                    chartDataBox.lines[i].far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
                }
                result.Add(chartDataBox);
            }
            return result;
        }

        // 遍历BoxEvents并转换的方法
        private static void ForeachBoxEvents(List<Data.ChartEdit.Event> editBoxEvent, List<Data.ChartData.Event> chartDataBoxEvent)
        {
            // 遍历ChartEdit的事件列表，创建并添加到ChartData的事件列表
            foreach (Data.ChartEdit.Event item in editBoxEvent)
            {
                chartDataBoxEvent.Add(new() { startTime = item.startBeats.ThisStartBPM, endTime = item.endBeats.ThisStartBPM, startValue = item.startValue, endValue = item.endValue, curve = item.curve.thisCurve });
            }
        }
        
    }
}