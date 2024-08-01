using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Dremu.Gameplay.Tool
{
    public static class EaseTypeEnumer
    {
        public enum EaseType
        {
            LINEAR,
            EASE_IN_SINE,
            EASE_OUT_SINE,
            EASE_IN_OUT_SINE,
            EASE_IN_QUAD,
            EASE_OUT_QUAD,
            EASE_IN_OUT_QUAD,
            EASE_IN_CUBIC,
            EASE_OUT_CUBIC,
            EASE_IN_OUT_CUBIC,
            EASE_IN_QUART,
            EASE_OUT_QUART,
            EASE_IN_OUT_QUART,
            EASE_IN_QUINT,
            EASE_OUT_QUINT,
            EASE_IN_OUT_QUINT,
            EASE_IN_EXPO,
            EASE_OUT_EXPO,
            EASE_IN_OUT_EXPO,
            EASE_IN_CIRC,
            EASE_OUT_CIRC,
            EASE_IN_OUT_CIRC,
            EASE_IN_ELASTIC,
            EASE_OUT_ELASTIC,
            EASE_IN_OUT_ELASTIC,
            EASE_IN_BACK,
            EASE_OUT_BACK,
            EASE_IN_OUT_BACK,
            EASE_IN_BOUNCE,
            EASE_OUT_BOUNCE,
            EASE_IN_OUT_BOUNCE,
        }


        /// <summary>
        /// 获取缓动函数
        /// </summary>
        /// <param name="length">缓动函数的总长度</param>
        /// <param name="count">缓动函数的总点数</param>
        /// <param name="easeType">缓动函数的类型，如LINEAR</param>
        /// <returns>缓动函数每一项的值</returns>
        /// <exception cref="Exception">缓动函数类型不正确抛出异常</exception>
        public static List<float> GetEase(float length, int count, EaseType easeType = EaseType.LINEAR)
        {
            List<float> NumList = new List<float>();
            switch (easeType)
            {
                #region EaseTypeEnuming

                case EaseType.LINEAR:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_SINE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_SINE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_SINE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_QUAD:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * Mathf.Pow(1f * i / count, 2f));
                    return NumList;
                case EaseType.EASE_OUT_QUAD:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * (1 - Mathf.Pow(1f - (1f * i / count), 2f)));
                    return NumList;
                case EaseType.EASE_IN_OUT_QUAD:
                    int mid = Mathf.CeilToInt(count / 2f);
                    for (float i = 0; i < mid; i++)
                        NumList.Add(length * 2 * Mathf.Pow(1f * i / count, 2f));
                    for (float i = mid; i <= count; i++)
                        NumList.Add(length * (1 - Mathf.Pow(-2f * i / count + 2, 2f) / 2));
                    return NumList;
                case EaseType.EASE_IN_CUBIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_CUBIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_CUBIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_QUART:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_QUART:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_QUART:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_QUINT:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_QUINT:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_QUINT:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_EXPO:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_EXPO:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_EXPO:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_CIRC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_CIRC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_CIRC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_BACK:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_BACK:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_BACK:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_ELASTIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_ELASTIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_ELASTIC:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_BOUNCE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_OUT_BOUNCE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                case EaseType.EASE_IN_OUT_BOUNCE:
                    for (float i = 0; i <= count; i++)
                        NumList.Add(length * i / count);
                    return NumList;
                default:
                    throw new System.Exception("EaseType not found");

                #endregion
            }

        }
    }
}