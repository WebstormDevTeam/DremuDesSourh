using System;
using System.Collections.Generic;
using Dremu.ChartEdit.Envelope;
using Dremu.Gameplay.Tool;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dremu.ChartEdit
{

    public class UIManager : Utils.Singleton.MonoBehaviourSingleton<UIManager>
    {
        public Texture2D mouseMoveTexture;
        public Vector2 mouseMoveHotspot;
        public Texture2D mouseUpDownTexture;
        public Vector2 mouseUpDownHotspot;
        [SerializeField] ContextMenu contextMenu;
        [SerializeField] Canvas uiCanvas;
        [SerializeField] CanvasScaler canvasScaler;
        [SerializeField] EditPanel editPanel;
        [SerializeField] VerticalTrackView view;
        [SerializeField] DropDownMenu menu;
        private float ScaleFactor => Screen.width / canvasScaler.referenceResolution.x * (1 - canvasScaler.matchWidthOrHeight) + Screen.height / canvasScaler.referenceResolution.y * canvasScaler.matchWidthOrHeight;

        private float _xPerBeat = 100;
        public float XperBeat
        {
            get => _xPerBeat;
            set
            {
                XperBeat = value;
                view.SetAllDirty();
            }
        }

        private float _FullTime;

        public float FullTime
        {
            get
            {
                return _FullTime;
            }
            set
            {
                _FullTime = value;
            }
        }

        public Canvas UICanvas { get => uiCanvas; }

        public void SetCursorType(CursorType type)
        {
            switch (type)
            {
                case CursorType.Normal: Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto); break;
                case CursorType.UpDown: Cursor.SetCursor(mouseUpDownTexture, mouseUpDownHotspot, CursorMode.Auto); break;
                case CursorType.Move: Cursor.SetCursor(mouseMoveTexture, mouseMoveHotspot, CursorMode.Auto); break;
            }
        }

        void Start()
        {
            FullTime = 20;
            SceneManager.LoadScene("GamePlay", LoadSceneMode.Additive);
            PositionHelper.width = 1920;
            PositionHelper.height = 1080;
            view.InitEnvelopes(new List<EnvelopeLine>{new EnvelopeLine(
                    new List<ControlNode> {
                        new ControlNode(
                            0,1,0.7f,CurveType.Expo
                        ),
                        new ControlNode(
                          1,0,0.7f,CurveType.Sine
                        ),
                        new ControlNode(
                            2,2,1,CurveType.Const
                        )

                    }
                ),new EnvelopeLine(
                    new List<ControlNode> {
                        new ControlNode(
                            0,1,0.7f,CurveType.Expo
                        ),
                        new ControlNode(
                          1,0,0.7f,CurveType.Sine
                        ),
                        new ControlNode(
                            2,2,1,CurveType.Const
                        )

                    }
                )});

        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(contextMenu.GetComponent<RectTransform>(), Input.mousePosition))
            {
                if (this.contextMenu.gameObject.activeSelf)
                {
                    contextMenu.gameObject.SetActive(false);
                }
            }
        }

        public void ShowContextMenu(Vector2 mousePos, List<ContextMenuItem> contextMenuItems)
        {
            contextMenu.GetComponent<RectTransform>().localPosition = mousePos / ScaleFactor;
            ActiveIfNeed(contextMenu.gameObject);
            contextMenu.SetButtonItems(contextMenuItems);
        }

        public void ShowEditingPanel(string text, string placeHolder, UnityAction<string> submitCallback, string Default = "")
        {
            ActiveIfNeed(editPanel.gameObject);
            this.editPanel.SetAttribute(text, placeHolder, submitCallback, Default);
        }

        public void ShowDropDown(Vector2 mousePos, Type enumType, UnityAction<object> reciver, int Default)
        {
            this.menu.GetComponent<RectTransform>().position = mousePos / ScaleFactor;
            ActiveIfNeed(menu.gameObject);
            this.menu.SetAttribute(enumType, reciver, Default);
        }



        private void ActiveIfNeed(GameObject obj)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        public static float GetBeatX()
        {
            return Instance.XperBeat;
        }
    }

    public enum CursorType
    {
        Normal,
        Move,
        UpDown
    }
}