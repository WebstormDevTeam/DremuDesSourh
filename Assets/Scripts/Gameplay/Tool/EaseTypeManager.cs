using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Dremu.Gameplay.Tool
{
    public static class EaseTypeManager
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
        
        //缓动函数的计算方式，除非你知道你在干什么否则千万别动
        ///<summary>计算缓动函数值</summary>
        ///<param name="x">自变量(0~1)</param>
        ///<param name="easeType">缓动函数类型</param>
        ///<returns>自变量对应的缓动函数值</returns>
        public static float GetEaseValue(float x, EaseType easeType = EaseType.LINEAR)
        {
            switch (easeType)
            {
                case EaseType.LINEAR:
                    return x;
                case EaseType.EASE_IN_SINE:
                    return 1 - Mathf.Cos((Mathf.PI / 2) * x);
                case EaseType.EASE_OUT_SINE:
                    return Mathf.Sin((Mathf.PI / 2) * x);
                case EaseType.EASE_IN_OUT_SINE:
                    return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
                case EaseType.EASE_IN_QUAD:
                    return Mathf.Pow(x, 2);
                case EaseType.EASE_OUT_QUAD:
                    return 1 - Mathf.Pow(1 - x, 2);
                case EaseType.EASE_IN_OUT_QUAD:
                    return x < 0.5 ? (2 * Mathf.Pow(x, 2)) : (1 - Mathf.Pow(-2 * x + 2, 2) / 2);
                case EaseType.EASE_IN_CUBIC:
                    return Mathf.Pow(x, 3);
                case EaseType.EASE_OUT_CUBIC:
                    return 1 - Mathf.Pow(1 - x, 3);
                case EaseType.EASE_IN_OUT_CUBIC:
                    return x < 0.5 ? (4 * Mathf.Pow(x, 3)) : (1 - Mathf.Pow(-2 * x + 2, 3) / 2);
                case EaseType.EASE_IN_QUART:
                    return Mathf.Pow(x, 4);
                case EaseType.EASE_OUT_QUART:
                    return 1 - Mathf.Pow(1 - x, 4);
                case EaseType.EASE_IN_OUT_QUART:
                    return x < 0.5 ? (8 * Mathf.Pow(x, 4)) : (1 - Mathf.Pow(-2 * x + 2, 4) / 2);
                case EaseType.EASE_IN_QUINT:
                    return Mathf.Pow(x, 5);
                case EaseType.EASE_OUT_QUINT:
                    return 1 - Mathf.Pow(1 - x, 5);
                case EaseType.EASE_IN_OUT_QUINT:
                    return x < 0.5 ? (16 * Mathf.Pow(x, 5)) : (1 - Mathf.Pow(-2 * x + 2, 5) / 2);
                case EaseType.EASE_IN_EXPO:
                    return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
                case EaseType.EASE_OUT_EXPO:
                    return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
                case EaseType.EASE_IN_OUT_EXPO:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return x < 0.5
                        ? Mathf.Pow(2, 20 * x - 10) / 2
                        : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
                case EaseType.EASE_IN_CIRC:
                    return 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));
                case EaseType.EASE_OUT_CIRC:
                    return Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
                case EaseType.EASE_IN_OUT_CIRC:
                    return x < 0.5
                        ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
                        : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
                case EaseType.EASE_IN_BACK:
                    return 2.70158f * Mathf.Pow(x, 3) - 1.70158f * Mathf.Pow(x, 2);
                case EaseType.EASE_OUT_BACK:
                    return 1 + 2.70158f * Mathf.Pow(x - 1, 3) + 1.70158f * Mathf.Pow(x - 1, 2);
                case EaseType.EASE_IN_OUT_BACK:
                    return x < 0.5
                        ? (3.59401f * 2 * x - 2.59401f) * Mathf.Pow(2 * x, 2) / 2
                        : ((3.59401f * (2 * x - 2) + 2.59401f) * Mathf.Pow(2 * x - 2, 2) + 2) / 2;
                case EaseType.EASE_IN_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * 2 * Mathf.PI / 3);
                case EaseType.EASE_OUT_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * 2 * Mathf.PI / 3) + 1;
                case EaseType.EASE_IN_OUT_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return x < 0.5
                        ? -Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((x * 20 - 11.125f) * 4 * Mathf.PI / 9) / 2
                        : Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((x * 20 - 11.125f) * 4 * Mathf.PI / 9) / 2 + 1;
                case EaseType.EASE_IN_BOUNCE:
                    if ((1 -  x) < 1 / 2.75f) return 1 - (7.5625f * Mathf.Pow((1 -  x), 2));
                    if ((1 -  x) < 2 / 2.75f) return 1 - (7.5625f * Mathf.Pow((1 -  x) - 1.5f / 2.75f, 2) + 0.75f);
                    if ((1 -  x) < 2.5f / 2.75f) return 1 - (7.5625f * Mathf.Pow((1 -  x) - 2.25f / 2.75f, 2) + 0.9375f);
                    return 1 - (7.5625f * Mathf.Pow((1 -  x) - 2.625f / 2.75f, 2) + 0.984375f);
                case EaseType.EASE_OUT_BOUNCE:
                    if (x < 1 / 2.75f) return 7.5625f * Mathf.Pow(x, 2);
                    if (x < 2 / 2.75f) return 7.5625f * Mathf.Pow(x - 1.5f / 2.75f, 2) + 0.75f;
                    if (x < 2.5f / 2.75f) return 7.5625f * Mathf.Pow(x - 2.25f / 2.75f, 2) + 0.9375f;
                    return 7.5625f * Mathf.Pow(x - 2.625f / 2.75f, 2) + 0.984375f;
                case EaseType.EASE_IN_OUT_BOUNCE:
                    if (x < 0.5)
                    {
                        if ((1 - 2 * x) < 1 / 2.75f) return (1 - (7.5625f * Mathf.Pow((1 - 2 * x), 2))) / 2;
                        if ((1 - 2 * x) < 2 / 2.75f) return (1 - (7.5625f * Mathf.Pow((1 - 2 * x) - 1.5f / 2.75f, 2) + 0.75f)) / 2;
                        if ((1 - 2 * x) < 2.5f / 2.75f) return (1 - (7.5625f * Mathf.Pow((1 - 2 * x) - 2.25f / 2.75f, 2) + 0.9375f)) / 2;
                        return (1 - (7.5625f * Mathf.Pow((1 - 2 * x) - 2.625f / 2.75f, 2) + 0.984375f)) / 2;
                    }
                    else
                    {
                        if ((2 * x - 1) < 1 / 2.75f) return (1 + (7.5625f * Mathf.Pow((2 * x - 1), 2))) / 2;
                        if ((2 * x - 1) < 2 / 2.75f) return (1 + (7.5625f * Mathf.Pow((2 * x - 1) - 1.5f / 2.75f, 2) + 0.75f)) / 2;
                        if ((2 * x - 1) < 2.5f / 2.75f) return (1 + (7.5625f * Mathf.Pow((2 * x - 1) - 2.25f / 2.75f, 2) + 0.9375f)) / 2;
                        return (1 + (7.5625f * Mathf.Pow((2 * x - 1) - 2.625f / 2.75f, 2) + 0.984375f)) / 2;
                    }
                default:
                    throw new System.Exception("EaseType not found");
            }
        }
        //缓动函数导数的计算方式，除非你知道你在干什么否则千万别动
        ///<summary>计算缓动函数导数值</summary>
        ///<param name="x">自变量(0~1)</param>
        ///<param name="easeType">缓动函数类型</param>
        ///<returns>自变量对应的缓动函数导数值</returns>
        public static float GetEaseDerivative(float x, EaseType easeType = EaseType.LINEAR)
        {
            switch (easeType)
            {
                case EaseType.LINEAR:
                    return 1;
                case EaseType.EASE_IN_SINE:
                    return Mathf.PI / 2 * Mathf.Sin((Mathf.PI / 2) * x);
                case EaseType.EASE_OUT_SINE:
                    return Mathf.PI / 2 * Mathf.Cos((Mathf.PI / 2) * x);
                case EaseType.EASE_IN_OUT_SINE:
                    return Mathf.PI / 2 * Mathf.Sin(Mathf.PI * x);
                case EaseType.EASE_IN_QUAD:
                    return 2 * x;
                case EaseType.EASE_OUT_QUAD:
                    return 2 * (1 - x);
                case EaseType.EASE_IN_OUT_QUAD:
                    return x < 0.5 ? (4 * x) : (4 * (1 - x));
                case EaseType.EASE_IN_CUBIC:
                    return 3 * Mathf.Pow(x, 2);
                case EaseType.EASE_OUT_CUBIC:
                    return 3 * Mathf.Pow(1 - x, 2);
                case EaseType.EASE_IN_OUT_CUBIC:
                    return x < 0.5 ? (12 * Mathf.Pow(x, 2)) : (12 * Mathf.Pow(1 - x, 2));
                case EaseType.EASE_IN_QUART:
                    return 4 * Mathf.Pow(x, 3);
                case EaseType.EASE_OUT_QUART:
                    return 4 * Mathf.Pow(1 - x, 3);
                case EaseType.EASE_IN_OUT_QUART:
                    return x < 0.5 ? (32 * Mathf.Pow(x, 3)) : (32 * Mathf.Pow(1 - x, 4));
                case EaseType.EASE_IN_QUINT:
                    return 5 * Mathf.Pow(x, 4);
                case EaseType.EASE_OUT_QUINT:
                    return 5 * Mathf.Pow(1 - x, 4);
                case EaseType.EASE_IN_OUT_QUINT:
                    return x < 0.5 ? (80 * Mathf.Pow(x, 4)) : (80 * Mathf.Pow(1 - x, 4));
                case EaseType.EASE_IN_EXPO:
                    return x == 0 ? 0 : 5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 10 * x - 9);
                case EaseType.EASE_OUT_EXPO:
                    return x == 1 ? 1 : 5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 1 - 10 * x);
                case EaseType.EASE_IN_OUT_EXPO:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return x < 0.5
                        ? 5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 20 * x - 9)
                        : 5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 11 - 20 * x);
                case EaseType.EASE_IN_CIRC:
                    return x / Mathf.Sqrt(1 - Mathf.Pow(x, 2));
                case EaseType.EASE_OUT_CIRC:
                    return (1 - x) / Mathf.Sqrt(1 - Mathf.Pow(1 - x, 2));
                case EaseType.EASE_IN_OUT_CIRC:
                    return x < 0.5
                        ? (2 * x / Mathf.Sqrt(1 - 4 * Mathf.Pow(x, 2)))
                        : (2 * (1 - x) / Mathf.Sqrt(1 - 4 * Mathf.Pow(1 - x, 2)));
                case EaseType.EASE_IN_BACK:
                    return 2.70158f * 3 * Mathf.Pow(x, 2) - 1.70158f * 2 * x;
                case EaseType.EASE_OUT_BACK:
                    return 2.70158f * 3 * Mathf.Pow((1 - x), 2) - 1.70158f * 2 * (1 - x);
                case EaseType.EASE_IN_OUT_BACK:
                    return x < 0.5
                        ? 3.59401f * 12 * Mathf.Pow(x, 2) - 2.59401f * 4 * x
                        : 3.59401f * 12 * Mathf.Pow(1 - x, 2) - 2.59401f * 4 * (1 - x);
                case EaseType.EASE_IN_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return -5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 10 * x - 9) * Mathf.Sin((20 * x - 21.5f) * Mathf.PI / 3)
                           - 5 * Mathf.PI * Mathf.Pow(2, 10 * x - 8) * Mathf.Cos((20 * x - 21.5f) * Mathf.PI / 3) / 3;
                case EaseType.EASE_OUT_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return -5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 10 * (1 - x) - 9) * Mathf.Sin((20 * (1 - x) - 21.5f) * Mathf.PI / 3)
                           - 5 * Mathf.PI * Mathf.Pow(2, 10 * (1 - x) - 8) * Mathf.Cos((20 * (1 - x) - 21.5f) * Mathf.PI / 3) / 3;
                case EaseType.EASE_IN_OUT_ELASTIC:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return x < 0.5
                        ? -5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 20 * x - 9) * Mathf.Sin((80 * x - 44.5f) * Mathf.PI / 9)
                          - 5 * Mathf.PI * Mathf.Pow(2, 20 * x - 7) * Mathf.Cos((80 * x - 44.5f) * Mathf.PI / 9) / 9
                        : -5 * Mathf.Log(2, MathF.E) * Mathf.Pow(2, 20 * (1 - x) - 9) * Mathf.Sin((80 * (1 - x) - 44.5f) * Mathf.PI / 9)
                          - 5 * Mathf.PI * Mathf.Pow(2, 20 * (1 - x) - 7) *
                          Mathf.Cos((80 * (1 - x) - 44.5f) * Mathf.PI / 9) / 9;
                case EaseType.EASE_IN_BOUNCE:
                    if (x < 1 / 2.75f) return 7.5625f * 2 * (1 - x);
                    if (x < 2 / 2.75f) return 7.5625f * 2 * ((1 - x) - 1.5f / 2.75f);
                    if (x < 2.5f / 2.75f) return 7.5625f * 2 * ((1 - x) - 2.25f / 2.75f);
                    return 7.5625f * 2 * ((1 - x) - 2.625f / 2.75f);
                case EaseType.EASE_OUT_BOUNCE:
                    if (x < 1 / 2.75f) return 7.5625f * 2 * x;
                    if (x < 2 / 2.75f) return 7.5625f * 2 * (x - 1.5f / 2.75f);
                    if (x < 2.5f / 2.75f) return 7.5625f * 2 * (x - 2.25f / 2.75f);
                    return 7.5625f * 2 * (x - 2.625f / 2.75f);
                case EaseType.EASE_IN_OUT_BOUNCE:
                    if (x < 0.5)
                    {
                        if ((1 - 2 * x) < 1 / 2.75f) return 7.5625f * (2 - 4 * x);
                        if ((1 - 2 * x) < 2 / 2.75f) return 7.5625f * (2 - 4 * x - 3 / 2.75f);
                        if ((1 - 2 * x) < 2.5f / 2.75f) return 7.5625f * (2 - 4 * x - 5 / 2.75f);
                        return 7.5625f * (2 - 4 * x - 5.25f / 2.75f);
                    }
                    else
                    {
                        if ((2 * x - 1) < 1 / 2.75f) return 7.5625f * (4 * x - 2);
                        if ((2 * x - 1) < 2 / 2.75f) return 7.5625f * (4 * x - 2 - 3 / 2.75f);
                        if ((2 * x - 1) < 2.5f / 2.75f) return 7.5625f * (4 * x - 2 - 5 / 2.75f);
                        return 7.5625f * (4 * x - 2 - 5.25f / 2.75f);
                    }
                default:
                    throw new System.Exception("EaseType not found");
            }
        }

        /// <summary>
        /// 获取缓动函数曲线
        /// </summary>
        /// <param name="length">缓动函数的总长度</param>
        /// <param name="count">缓动函数的总点数</param>
        /// <param name="easeType">缓动函数的类型，如LINEAR</param>
        /// <returns>缓动函数曲线每一项的值</returns>
        public static List<float> GetEaseLine(float length, int count, EaseType easeType = EaseType.LINEAR)
        {
            List<float> numList = new List<float>();
            for (int i = 0; i < count; i++)
                numList.Add(length * GetEaseValue(1f * i / (count - 1), easeType));
            return numList;
        }
    }
}