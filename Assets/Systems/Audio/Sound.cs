using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameJam
{
    [System.Serializable]
    public class Sound
    {
        //currently unused, might be worth expanding upon later though.

        // public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 0.5f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop;
    }
}
