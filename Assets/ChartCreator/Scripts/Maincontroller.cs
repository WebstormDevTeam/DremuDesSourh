using System;
using ChartCreator.Scripts.Tools;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace ChartCreator.Scripts
{
    public class Maincontroller : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;


        public static Maincontroller Instance;
        private void Awake()
        {
            Instance = this;
        }

        private Button _createButton;
        private Button _closeButton;

        /// <summary>
        /// 用来设定组件的代码
        /// </summary>
        private void SetGetElements()
        {
            //获取代码组件的方法
            _createButton = ElementTools.GetElementById<Button>(ref uiDocument, "NewChartButton");
            _createButton.clicked += () =>
            {
                Debug.Log("OnClick");
            };
            _closeButton = ElementTools.GetElementById<Button>(ref uiDocument,"CloseChartButton");
            _closeButton.clicked += () =>
            {
                //这里释放读写文件的
                Debug.Log("close this ChartFile");
            };
        }
        
        private void Start()
        {
            SetGetElements();
        }
    }
}