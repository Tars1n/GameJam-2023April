using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        [SerializeField] private AudioSource _musicSource, _effectsSource;

        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                CreateAudioSources();
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

        public void PlaySound(AudioClip clip)
        {
            _effectsSource.PlayOneShot(clip);
        }
    }
}
