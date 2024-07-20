using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dremu.Gameplay.Tool {

    public interface RecyclableObject {

        /// <summary>
        /// 在物体从对象池中取出时会自动调用，用于初始化物体。
        /// </summary>
        void OnInitialize();

        /// <summary>
        /// 在物体放回对象池时会自动调用。
        /// </summary>
        void OnRecycle();
    }

    /// <summary>
    /// 通用对象池
    /// </summary>
    /// <typeparam name="T">Unity组件</typeparam>
    public sealed class StandardObjectPool<T> where T : MonoBehaviour, RecyclableObject {
        private T instance;

        private Queue<T> objList = new Queue<T>();

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="Instance">物体</param>
        public StandardObjectPool( T Instance ) {
            instance = Instance;
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="Instance">物体</param>
        /// <param name="Count">初始物体数量</param>
        public StandardObjectPool( T Instance, int Count) {
            instance = Instance;
            for (int i = 0; i < Count; i++) {
                T obj = UnityEngine.Object.Instantiate(instance);
                obj.gameObject.SetActive(false);
                objList.Enqueue(obj);
            }
        }

        /// <summary>
        /// 从对象池中获取一个物体
        /// </summary>
        /// <returns>获取的物体</returns>
        public T GetObject() {
            T obj;
            if (objList.Count > 0)
                obj = objList.Dequeue();
            else
                obj = UnityEngine.Object.Instantiate(instance);
            obj.OnInitialize();
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 将物体放回对象池
        /// </summary>
        /// <param name="Object">要放回的物体</param>
        public void ReturnObject( T Object ) {
            Object.gameObject.SetActive(false);
            Object.OnRecycle();
            objList.Enqueue(Object);
        }

    }

}
