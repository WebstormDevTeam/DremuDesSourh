
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Form.LoadGamePlayer
{
    public class LoadGamePlayer: MonoBehaviour
    {
        [SerializeField] private string gamePlayerSceneName;//Название сцены, используемой для чарта

        private void Start()
        {
            LoadChartPlayer(gamePlayerSceneName);
        }

        public void LoadChartPlayer(string gamePlayerName)
        {
            LoadSceneAsynchronously(gamePlayerName);
        }
        
        private IEnumerator LoadSceneAsynchronously(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
            // 显示加载进度
            // loadingProgressBar.gameObject.SetActive(true);
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                // loadingProgressBar.value = progress;
                yield return null;
            }
        }
    }
}