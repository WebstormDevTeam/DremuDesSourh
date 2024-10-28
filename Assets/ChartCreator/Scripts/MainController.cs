using System;
using System.Drawing;
using ChartCreator.Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using Color = UnityEngine.Color;

namespace ChartCreator.Scripts
{
    public class Maincontroller : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;

        /// <summary>
        /// 非主UI文档部分
        /// </summary>

        public static Maincontroller Instance;
        private Button _createButton;
        private Button _closeButton;
        private Button _WTFButton;
        private ScrollView _scrollView;


        private void Awake()
        {
            Instance = this;
        }


        /// <summary>
        /// 用来设定组件的代码
        /// </summary>
        private void GetUIElement()
        {
            //获取代码组件
            _createButton = ElementTools.GetElementById<Button>(ref uiDocument, "NewChartButton");
            _closeButton = ElementTools.GetElementById<Button>(ref uiDocument,"CloseChartButton");
            _WTFButton = ElementTools.GetElementById<Button>(ref uiDocument,"WTFButton");
            VisualElement timeline = ElementTools.GetElementByClass<VisualElement>(ref uiDocument, "timeline-class");
            _scrollView = ElementTools.GetElementById<ScrollView>(ref timeline, "ScrollView");
        }

        private void SetElementsState()
        {
            _createButton.clicked += () =>
            {
                Debug.Log("OnClick");
            };
            _closeButton.clicked += () =>
            {
                //这里释放读写文件的
                Debug.Log("close this ChartFile");
            };
            _WTFButton.clicked += () =>
            {
                for(int i=0;i<=10;i++)
                {
                    Debug.Log("W");
                    Debug.Log("T");
                    Debug.Log("F");
                }
            };
            VisualElement beatLine = new VisualElement
            {
                style =
                {
                    width = 2,
                    height = 100,
                    backgroundColor = new StyleColor(Color.black)
                }
            };
            VisualElement space = new VisualElement
            {
                style =
                {
                    width = 10,
                    height = new StyleLength(StyleKeyword.Auto)
                }
            };
            for (int i = 0; i < 10; i++)
            {
                _scrollView.Add(beatLine);
                _scrollView.Add(space);
                Debug.Log($"Now is creating num:{i}");
            }

        }

        private void Start()
        {
            GetUIElement();
            SetElementsState();

        }
    }
}