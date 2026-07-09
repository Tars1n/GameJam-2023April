using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace GameJam
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider mainVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider SFXVolumeSlider;

        void Awake()
        {
            if (audioMixer == null)
            {
                Debug.Log("AudioMixerManager does not have AudioMixer plugged in");
                return;
            }
            //AdjustLevels();

        }

        public void SetupMusicMixer(AudioSource _musicSource)
        {
            if (_musicSource == null)
            {
                Debug.Log("AudioMixerManager did not receive valid music source");
                return;
            }
            
            SetupMixer(_musicSource, "Music");  
            
            _musicSource.loop = true;
            SetMusicVolume(musicVolumeSlider.value);

            Debug.Log("Music Source mixer completed setup.");
        }

        public void SetupSFXMixer(AudioSource _SFXSource)
        {
            if (_SFXSource == null)
            {
                Debug.Log("AudioMixerManager did not receive valid SFX source");
                return;
            }
            
            SetupMixer(_SFXSource, "SoundFX");            
            SetSoundFXVolume(SFXVolumeSlider.value);

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
            audioMixer.SetFloat("masterVolume", value);
        }

        public void SetSoundFXVolume(float level)
        {
            float value = ConvertDecibelValue(level);
            audioMixer.SetFloat("soundFXVolume", value);
        }

        public void SetMusicVolume(float level)
        {
            float value = ConvertDecibelValue(level);
            audioMixer.SetFloat("musicVolume", value);
        }

        private float ConvertDecibelValue(float level)
        {
            if (level <-80 || level > 0)
            {
                Debug.Log("Attempting to set AudioMixer outside of -80 to 0 dB mix range, defaulting to -80 dB (muted).");
                level = -80;
            }
            //AudioMixer is using decibel grading, -80 min value to 0 max value. This converts from a linear value to a logrithmic one.
            return Mathf.Log10(level) * 20f;
        }
    }
}
