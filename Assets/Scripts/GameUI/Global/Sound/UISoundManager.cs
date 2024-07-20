using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine;
using Utils.Helper;
using Utils.Singleton;

namespace GameUI.Global.Sound
{
    public class UISoundManager : MonoBehaviourSingleton<UISoundManager>
    {
        [SerializeField] private AudioSource soundSource;
        [OdinSerialize] public Dictionary<string, AudioClip> SoundDict { get; private set; } = new();

        public static async void Play(string soundKey, float soundVolume = 1f)
        {
            if (!Instance)
            {
                Debug.LogError("UISoundManager : Isn't Active");
                return;
            }

            var clip = await Instance.LoadSound(soundKey);
            Instance.soundSource.PlayOneShot(clip, soundVolume);
        }

        public static void Play(params string[] soundKeys)
        {
            foreach (var key in soundKeys)
            {
                Play(key);
            }
        }

        private async Task<AudioClip> LoadSound(string soundKey)
        {
            if (SoundDict.ContainsKey(soundKey))
            {
                return SoundDict[soundKey];
            }

            var request = Resources.LoadAsync<AudioClip>(@$"Sounds/{soundKey}");
            await request.ToTask();
            
            var clip = request.asset as AudioClip;
            
            if (gameObject && clip && !SoundDict.ContainsKey(soundKey)) 
                SoundDict.Add(soundKey, clip);
            
            if (!clip) Debug.LogError($"UISoundManager : Not Found Sound {soundKey}");
            else Debug.Log($"UISoundManager : Found Sound {soundKey}");
            
            return clip;
        }
    }
}