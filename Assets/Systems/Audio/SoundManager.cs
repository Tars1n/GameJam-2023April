using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameJam
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        private AudioLibrary _audioLib;
        private MusicLibrary _musicLib;
        public AudioLibrary Lib => _audioLib;
        public MusicLibrary MusicLib => _musicLib;
        private AudioClip _currentBGM;
        private AudioMixerManager _audioMixerManager;

        [SerializeField] private AudioSource _musicSource, _effectsSource;

        private void Awake() {
            if (Instance == null)
            {
                Instance = this;
                CreateAudioSources();
                CreateMixerManager();
                FindAudioLibrary();
                _audioMixerManager = FindAnyObjectByType<AudioMixerManager>();
                if (_audioMixerManager == null)
                {
                    Debug.Log("Could not find AudioMixerManager");
                    return;
                }
                DontDestroyOnLoad(gameObject);
                Debug.Log("Sound Manager Instanced");
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
            //_musicSource.outputAudioMixerGroup = _audioMixerManager.GetComponent<AudioMixer>().FindMatchingGroups("Music")[1];
            _musicSource.loop = true;

            go = new GameObject();
            go.name = "Effect Source";
            go.transform.SetParent(transform);
            _effectsSource = go.AddComponent<AudioSource>();
        } 

        private void CreateMixerManager()
        {
            _audioMixerManager = gameObject.AddComponent<AudioMixerManager>();
        }

        private void FindAudioLibrary()
        {
            _audioLib = GameMaster.Instance.ReferenceManager.LevelManager.AudioLibrary;
            if (_audioLib == null)
            {
                Debug.LogWarning("No AudioLibrary asset found on LevelManager.");
            }
            _musicLib = GameMaster.Instance.ReferenceManager.LevelManager.MusicLibrary;
            if (_musicLib == null)
            {
                Debug.LogWarning("No MusicLibrary asset found on LevelManager");
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

        public void TryMusicTrack(AudioClip track)
        {
            if (track == null)
            {
                Debug.LogWarning("TryMusicTrack was called with a null AudioClip.");
                return;
            }
            if (track == _currentBGM)
            {
                //MusicSource is already playing the current song, let it continue.
                return;
            }
            SetMusicTrack(track);
        }
        
        public void SetMusicTrack(AudioClip track)
        {
            if (track == null)
            {
                Debug.LogWarning("PlaySound was called with a null AudioClip.");
                return;
            }
            _musicSource.PlayOneShot(track);
            _currentBGM = track;
        }
    }
}
