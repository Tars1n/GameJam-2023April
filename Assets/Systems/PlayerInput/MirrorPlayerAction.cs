using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;

namespace GameJam.PlayerInput
{
    public class MirrorPlayerAction : MonoBehaviour
    {
        [SerializeField] private Vector3Int _mirrorOrigin;
        [SerializeField] private bool _mirrorX = true;
        [SerializeField] private bool _mirrorY = false;
    }
}
