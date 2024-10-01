using System;
using ChartCreator.Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

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
            var timeline = ElementTools.GetElementByClass<VisualElement>(ref uiDocument, "timeline-class");
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
        }

        private void Start()
        {
            GetUIElement();
            SetElementsState();

        }
    }
}