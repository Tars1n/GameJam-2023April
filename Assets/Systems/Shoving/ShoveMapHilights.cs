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

        public void TryRenderShoveEntity(TileNode sourceTile, TileNode tileBeingPushed)
        {
            if (CanShoveThisTile(tileBeingPushed))
            {
                _mouseMap.SetTile(tileBeingPushed.GridCoordinate, GetPushTile(sourceTile.GridCoordinate, tileBeingPushed.GridCoordinate));
            }
        }
        public bool CanShoveThisTile(TileNode tileBeingPushed)
        {
            if (tileBeingPushed.Entities.Count == 0) return false;
            foreach (EntityBase entity in tileBeingPushed.Entities)
            {
                if (entity.GetComponent<Shovable>() != null)
                {
                    return true;
                }
            }
            return false;
        }
        private TileBase GetPushTile(Vector3Int sourceCoords, Vector3Int targetCoords)
        {
            Vector3Int axialSourceCoords = _mapManager.CastOddRowToAxial(sourceCoords);
            Vector3Int axialTargetCoords = _mapManager.CastOddRowToAxial(targetCoords);
            Vector3Int axialDifference = axialTargetCoords - axialSourceCoords;
            int indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 1)) indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 0)) indexOfTileBase = 1;
            if ((axialDifference.x == 0) && (axialDifference.y == -1)) indexOfTileBase = 2;
            if ((axialDifference.x == 1) && (axialDifference.y == -1)) indexOfTileBase = 3;
            if ((axialDifference.x == 1) && (axialDifference.y == 0)) indexOfTileBase = 4;
            if ((axialDifference.x == 0) && (axialDifference.y == 1)) indexOfTileBase = 5;
            Debug.Log($"shoving in direction " + indexOfTileBase);
            return (_shoveTileHilight[indexOfTileBase]);
        }
        public void ShoveThisTile(TileNode sourceOfShove, TileNode targetOfShove)
        {
            Debug.Log($"shoving from {sourceOfShove} to " + targetOfShove);
        }
    }
}
