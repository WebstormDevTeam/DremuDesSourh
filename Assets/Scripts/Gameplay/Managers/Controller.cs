using UnityEngine;
using UnityEngine.UI;

namespace Dremu.Gameplay.Manager
{
    public class Controller:MonoBehaviour,GameplayState
    {
        #region GetDataAndElements
        [SerializeField, Min(0)] private float currentTime;

        [SerializeField] private Button pauseButton;
        [SerializeField] private Text songName;
        #endregion

        private static MainController _instance;

        public delegate void Callback();

        public string jsonPath;
        private static bool _pauseState;


        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Continue()
        {
            throw new System.NotImplementedException();
        }

        public void Restart()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }
    }
}