#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor
{
    public static class SceneTools
    {
        [MenuItem("Scene Tools/Into MainUI")]
        private static void IntoEntry()
        {
            IntoScene("MainUIScene");
        }
    
        [MenuItem("Scene Tools/Into ChapterSelect")]
        private static void IntoChapterSelect()
        {
            IntoScene("ChapterSelectScene");
        }
    
        [MenuItem("Scene Tools/Into LevelSelect")]
        private static void IntoLevelSelect()
        {
            IntoScene("LevelSelectScene");
        }
    
        [MenuItem("Scene Tools/Into Gameplay")]
        private static void IntoGameScene()
        {
            IntoScene("Gameplay");
        }
    
        [MenuItem("Scene Tools/Into Result")]
        private static void IntoResultShow()
        {
            IntoScene("ResultScene");
        }
    
        [MenuItem("Scene Tools/Into Settings")]
        private static void IntoSettings()
        {
            IntoScene("SettingsScene");
        }

        private static void IntoScene(string name)
        {
            if (EditorApplication.isPlaying)
            {
                SceneManager.LoadSceneAsync($"Scenes/{name}");
                return;
            }
            EditorSceneManager.OpenScene($"Assets/Scenes/{name}.unity");
        }
    }
}

#endif
