using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "MusicLibrary", menuName = "Audio/MusicLibrary")]
    [System.Serializable]
    public class MusicLibrary : ScriptableObject
    {
        public AudioClip BGM01;
        public AudioClip BGM02;
    }
}
