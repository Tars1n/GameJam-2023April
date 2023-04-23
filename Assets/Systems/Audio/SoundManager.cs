using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        private AudioLibrary _audioLib;
        public AudioLibrary Lib => _audioLib;
        [SerializeField] private AudioSource _musicSource, _effectsSource;

        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                CreateAudioSources();
                FindAudioLibrary();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void CreateAudioSources()
        {
            GameObject go = new GameObject();
            go.name = "Music Source";
            go.transform.SetParent(transform);
            _musicSource = go.AddComponent<AudioSource>();

            go = new GameObject();
            go.name = "Effect Source";
            go.transform.SetParent(transform);
            _effectsSource = go.AddComponent<AudioSource>();
        } 

        private void FindAudioLibrary()
        {
            _audioLib = GameMaster.Instance.ReferenceManager.LevelManager.AudioLibrary;
            if (_audioLib == null)
            {
                Debug.LogWarning("No AudioLibrary asset found on LevelManager.");
            }
        }

        public void PlaySound(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("PlaySound was called with a null AudioClip.");
                return;
            }
            _effectsSource.PlayOneShot(clip);
        }
    }
}
