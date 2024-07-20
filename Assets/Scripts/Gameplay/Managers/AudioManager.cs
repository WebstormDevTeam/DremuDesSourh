using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dremu.Gameplay.Manager {

    public class AudioManager : MonoBehaviour {

        [SerializeField] AudioSource _MusicAudioSource;
        [SerializeField] AudioSource _SoundAudioSource;

        public static float MusicVolume {
            get => Instance._MusicAudioSource.volume;
            set { Instance._MusicAudioSource.volume = value; }
        }
        public static float SoundVolume {
            get => Instance._SoundAudioSource.volume;
            set { Instance._SoundAudioSource.volume = value; }
        }
        public static bool IsPlaying => Instance._MusicAudioSource.isPlaying;


        public static float Time => 1f * Instance._MusicAudioSource.timeSamples / Instance._MusicAudioSource.clip.frequency;

        public static AudioManager Instance;

        private void Awake() {
            Instance = this;
        }

        public static void PlayMusic( AudioClip Clip ) {
            Instance._MusicAudioSource.clip = Clip;
            Instance._MusicAudioSource.Play();
        }

        public static void PlayMusic( string Path ) {
            Instance._MusicAudioSource.clip = Resources.Load<AudioClip>(Path);
            Instance._MusicAudioSource.Play();
        }

        public static void PlaySound( AudioClip Clip ) {
            Instance._SoundAudioSource.PlayOneShot(Clip);
        }

        public static void StopMusic() {
            Instance._MusicAudioSource.Stop();
        }

        public static void PauseMusic() {
            Instance._MusicAudioSource.Pause();
        }

        public static void UnPauseMusic() {
            Instance._MusicAudioSource.UnPause();
        }

    }

}
