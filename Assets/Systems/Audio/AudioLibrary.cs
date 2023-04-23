using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Library")]
    [System.Serializable]
    public class AudioLibrary : ScriptableObject
    {
        public AudioClip EntityHop;
        public AudioClip EntityLanding;
        public AudioClip StartShove;
        public AudioClip Sliding;
        public AudioClip Collision;
        public AudioClip MonsterFallIntoPit;
        public AudioClip PlayerFallIntoPit;
        public AudioClip CultistAttackedPlayer;
        public AudioClip TrapActivated;
    }
}
