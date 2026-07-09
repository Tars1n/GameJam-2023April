using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GameJam.Audio
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;

        const string MIXER_MASTER = "masterVolume";
        const string MIXER_MUSIC = "musicVolume";
        const string MIXER_SFX = "soundFXVolume";

        void Awake()
        {
            if (audioMixer == null)
            {
                Debug.Log("AudioMixerManager does not have AudioMixer plugged in");
                return;
            }
            if (mainVolumeSlider == null || musicVolumeSlider == null || SFXVolumeSlider == null)
            {
                Debug.Log("AudioMixerManager sliders not set.");
            }
        }

        public void SetupMusicMixer(AudioSource _musicSource)
        {
            if (_musicSource == null)
            {
                Debug.Log("AudioMixerManager did not receive valid music Audio Source");
                return;
            }
            
            SetupMixer(_musicSource, "Music");  
            
            _musicSource.loop = true;
            Debug.Log("Music Source mixer completed setup.");
        }

        public void SetupSFXMixer(AudioSource _SFXSource)
        {
            if (_SFXSource == null)
            {
                Debug.Log("AudioMixerManager did not receive valid SFX Audio Source");
                return;
            }
            
            SetupMixer(_SFXSource, "SoundFX");            
            Debug.Log("SFX Source mixer completed setup.");

        }

        private void SetupMixer (AudioSource _audioSource, string _mixerName)
        {
            AudioMixerGroup[] matchingGroups = audioMixer.FindMatchingGroups(_mixerName);
            if (matchingGroups.Length > 0 )
            {
                _audioSource.outputAudioMixerGroup = matchingGroups[0];
                Debug.Log($"{_audioSource} set up with {_mixerName} mixer.");
            }
            else
            {
                Debug.Log($"Could not find valid {_mixerName} mixer for {_audioSource}");
            }
        }

        public void AdjustLevels()
        {
            SetMasterVolume(mainVolumeSlider.value);
            SetMusicVolume(musicVolumeSlider.value);
            SetSoundFXVolume(SFXVolumeSlider.value);
        }

        public void SetMasterVolume(float level)
        {
            float value = ConvertDecibelValue(level);
            audioMixer.SetFloat(MIXER_MASTER, value);
        }

        public void SetMusicVolume(float level)
        {
            float value = ConvertDecibelValue(level);
            audioMixer.SetFloat(MIXER_MUSIC, value);
        }

        public void SetSoundFXVolume(float level)
        {
            float value = ConvertDecibelValue(level);
            audioMixer.SetFloat(MIXER_SFX, value);
        }

        private float ConvertDecibelValue(float level)
        {
            float val = Mathf.Clamp(level, 0.0001f, 1f);
            val = Mathf.Log10(val) * 20;
            Debug.Log($"Volume set to {val}.");
           
            //AudioMixer is using decibel grading, -80 min value to 0 max value. This converts from a linear value to a logrithmic one.
            return val;
        }
    }
}
