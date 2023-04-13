using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Entity.Shoving;

namespace GameJam.Map
{
    public class ShoveMapHilights : MonoBehaviour
    {
        private MapManager _mapManager;
        // private MapInteractionManager _mapInteractionManager;
        private Tilemap _mouseMap;
        [SerializeField] private TileBase[] _shoveTileHilight;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            // _mapInteractionManager = GetComponent<MapInteractionManager>();
            _mouseMap = _mapManager.MouseInteractionTilemap;
        }

        public void TryRenderShoveEntity(TileNode tile)
        {
            if (tile.Entities.Count == 0) return;
            foreach (EntityBase entity in tile.Entities)
            {
                if (entity.GetComponent<Shovable>() != null)
                {
                    _mouseMap.SetTile(tile.GridCoordinate, _shoveTileHilight[0]);
                }
            }
        }
    }
}
