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

        public Button createButton;

        private void getElements()
        {
            createButton = ElementTools.GetElementById<Button>(ref uiDocument, "NewChartButton");
            createButton.clicked += () =>
            {
                Debug.Log("OnClick");
            };
        }
        
        private void Start()
        {
            getElements();
        }
    }
}