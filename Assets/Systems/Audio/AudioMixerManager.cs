using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements.Experimental;

namespace GameJam
{
    public class AudioMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;

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
