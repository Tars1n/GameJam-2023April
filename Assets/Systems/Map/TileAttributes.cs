using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    [CreateAssetMenu(fileName = "Tile_", menuName = "Game Asset/New TileAttributes")]
    [System.Serializable]
    public class TileAttributes : ScriptableObject
    {
        public TileBase[] Tiles;
        public bool IsSelectable;
        public bool IsWalkable;
        public bool IsPitTile;
        public bool OcclusionLayer;
        
    }
}
