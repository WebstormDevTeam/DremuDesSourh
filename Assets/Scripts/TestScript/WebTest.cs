using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace TestScript
{
    public class WebTest : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private IEnumerator CallToServer()
        {
            string url = "http://localhost:8888/UnityWeb";
            string data = "Message From Unity";

            UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type","text/plain");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }

        private IEnumerator Start()
        {
            btn.onClick.AddListener(() =>
            {
                StartCoroutine(CallToServer());
            });



            yield return null;
        }
    }
}