using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils.Singleton
{
    public abstract class MonoBehaviourSingleton<T> : SerializedMonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public static string SceneToGo { get; private set; } = "Scenes/MainUIScene";
        
        public static void SetSceneToGo(string sceneName) => SceneToGo = sceneName;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<T>();
            }
            else Destroy(this);
            OnAwake();
        }
    
        protected virtual void OnAwake() {}
    }
}
