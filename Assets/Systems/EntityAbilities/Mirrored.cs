using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity.Abilities
{
    public class Mirrored : MonoBehaviour
    {
        [SerializeField] private EntityCharacter _mirrorEntityX;
        public EntityCharacter MirrorEntityX => _mirrorEntityX;
        [SerializeField] private EntityCharacter _mirrorEntityY;
        public EntityCharacter MirrorEntityY => _mirrorEntityY;
    }
}
