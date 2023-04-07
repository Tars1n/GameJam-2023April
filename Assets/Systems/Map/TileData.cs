using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    [CreateAssetMenu(fileName = "Tile_", menuName = "Game Asset/New TileData")]
    [System.Serializable]
    public class TileData : ScriptableObject
    {
        public TileBase[] Tiles;
        public bool IsWalkable;
        public bool IsTrapTile;
        
    }
}
