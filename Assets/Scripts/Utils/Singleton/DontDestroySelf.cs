using System;
using UnityEngine;

namespace Utils.Singleton
{
    [DisallowMultipleComponent]
    public class DontDestroySelf : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
