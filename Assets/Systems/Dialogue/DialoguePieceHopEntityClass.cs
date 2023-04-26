using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceHopEntityClass : DialoguePieceClass
    {
        [SerializeField] private EntityBase _entity;
        public EntityBase Entity => _entity;
        [SerializeField] private Vector3Int _tileToHopTo;
        public Vector3Int TileToHopTo => _tileToHopTo;
        [SerializeField] private float _durationOfHop;
        public float DurationOfHop => _durationOfHop;
        [SerializeField] private bool _slamAtEnd;
        public bool SlamAtEnd => _slamAtEnd;
    }
}
