using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ChartCreator.Scripts.Tools
{
    public class ElementTools
    {
        /// <summary>
        /// 从ID获取UI Object组件
        /// </summary>
        /// <param name="uiDocument">对应的UI文档</param>
        /// <param name="elementId">组件ID</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>该对象</returns>
        /// <exception cref="Exception">无法找到该组件</exception>
        public static T GetElementById<T>(ref UIDocument uiDocument, string elementId) where T : VisualElement
        {
            T element = uiDocument.rootVisualElement.Query<T>(elementId);
            if (element == null)
            {
                throw new Exception("未能找到该组件，请检查该ID选择器是否存在");
            }
            return element;
        }

        /// <summary>
        /// 从Class获取UI Object组件
        /// </summary>
        /// <param name="uiDocument">对应的UI文档</param>
        /// <param name="className">对应的类名</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>组件对象</returns>
        /// <exception cref="Exception">无法找到该组件</exception>
        public static T GetElementByClass<T>(ref UIDocument uiDocument, string className) where T : VisualElement
        {
            T element = uiDocument.rootVisualElement.Query<T>(className: className);
            if (element == null)
            {
                throw new Exception("无法找到该组件，话检查该Class选择器是否存在");
            }
            return element;
        }


    }
}