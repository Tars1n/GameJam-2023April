using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileClassStorage : MonoBehaviour
    {
        [SerializeField] private GameObject _unit;
        public GameObject Unit { get => _unit; set => _unit = value; }
        [SerializeField] private Vector2 _prevTileCoord;
        public Vector2 PrevTileCoord { get => _prevTileCoord; set => _prevTileCoord = value; }        
    }
}
