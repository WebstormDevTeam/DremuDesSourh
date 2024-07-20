using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class Utility
    {
        public static void ForEach<T>(this IList<T> array, Action<T> action)
        {
            for (var i = 0; i < array.Count; i++)
            {
                action?.Invoke(array[i]);
            }
        }

        public static void ForEach<T>(this IList<T> list, Action<T, int> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                action?.Invoke(list[i], i);
            }
        }
    }
}
