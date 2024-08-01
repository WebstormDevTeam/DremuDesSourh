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

    // UIManager类，继承自Utils.Singleton.MonoBehaviourSingleton<UIManager>，用于管理UI相关的操作
    public class UIManager : Utils.Singleton.MonoBehaviourSingleton<UIManager>
    {
        // 鼠标移动纹理和热点
        public Texture2D mouseMoveTexture;
        public Vector2 mouseMoveHotspot;
        // 鼠标上下移动纹理和热点
        public Texture2D mouseUpDownTexture;
        public Vector2 mouseUpDownHotspot;
        // 上下文菜单
        [SerializeField] ContextMenu contextMenu;
        // UI画布
        [SerializeField] Canvas uiCanvas;
        // 画布缩放器
        [SerializeField] CanvasScaler canvasScaler;
        // 编辑面板
        [SerializeField] EditPanel editPanel;
        // 垂直轨道视图
        [SerializeField] VerticalTrackView view;
        // 下拉菜单
        [SerializeField] DropDownMenu menu;
        
        // 计算缩放因子
        private float ScaleFactor => Screen.width / canvasScaler.referenceResolution.x * (1 - canvasScaler.matchWidthOrHeight) + Screen.height / canvasScaler.referenceResolution.y * canvasScaler.matchWidthOrHeight;

        // 每拍对应的X轴距离
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

        // 完整时间
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

        // 获取UI画布
        public Canvas UICanvas { get => uiCanvas; }

        // 设置光标类型
        public void SetCursorType(CursorType type)
        {
            switch (type)
            {
                case CursorType.Normal: Cursor.SetCursor(null, new Vector2(0, 0), CursorMode.Auto); break;
                case CursorType.UpDown: Cursor.SetCursor(mouseUpDownTexture, mouseUpDownHotspot, CursorMode.Auto); break;
                case CursorType.Move: Cursor.SetCursor(mouseMoveTexture, mouseMoveHotspot, CursorMode.Auto); break;
            }
        }

        // 启动时执行的初始化操作
        void Start()
        {
            FullTime = 20;
            SceneManager.LoadScene("GamePlay", LoadSceneMode.Additive);//加载GamePlayer的场景
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

        // 每帧更新时执行的操作
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

        // 显示上下文菜单
        public void ShowContextMenu(Vector2 mousePos, List<ContextMenuItem> contextMenuItems)
        {
            contextMenu.GetComponent<RectTransform>().localPosition = mousePos / ScaleFactor;
            ActiveIfNeed(contextMenu.gameObject);
            contextMenu.SetButtonItems(contextMenuItems);
        }

        // 显示编辑面板
        public void ShowEditingPanel(string text, string placeHolder, UnityAction<string> submitCallback, string Default = "")
        {
            ActiveIfNeed(editPanel.gameObject);
            this.editPanel.SetAttribute(text, placeHolder, submitCallback, Default);
        }

        // 显示下拉菜单
        public void ShowDropDown(Vector2 mousePos, Type enumType, UnityAction<object> reciver, int Default)
        {
            this.menu.GetComponent<RectTransform>().position = mousePos / ScaleFactor;
            ActiveIfNeed(menu.gameObject);
            this.menu.SetAttribute(enumType, reciver, Default);
        }

        // 如果对象未激活，则激活它
        private void ActiveIfNeed(GameObject obj)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        // 获取每拍的X轴距离
        public static float GetBeatX()
        {
            return Instance.XperBeat;
        }
    }

    // 光标类型枚举
    public enum CursorType
    {
        Normal,
        Move,
        UpDown
    }
}
