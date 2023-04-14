using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Entity.Shoving;

namespace GameJam.Map
{
    public class MapShoveInteraction : MonoBehaviour
    {
        private bool _debugLogs = false;
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

        public void TryRenderShoveHilight(TileNode sourceTile, TileNode tileBeingPushed)
        {
            if (EntityOnThisTileThatCanBeShoved(tileBeingPushed))
            {
                _mouseMap.SetTile(tileBeingPushed.GridCoordinate, GetPushHilight(sourceTile.GridCoordinate, tileBeingPushed.GridCoordinate));
            }
        }
        public bool EntityOnThisTileThatCanBeShoved(TileNode tileBeingPushed)
        {
            if ((tileBeingPushed.Entities == null) || (tileBeingPushed.Entities.Count == 0)) return false;
            foreach (EntityBase entity in tileBeingPushed.Entities)
            {
                if (entity == null) { continue; }
                if (entity?.GetComponent<Shovable>() != null)
                {
                    return true;
                }
            }
            return false;
        }
        private TileBase GetPushHilight(Vector3Int sourceCoords, Vector3Int targetCoords)
        {
            Vector3Int axialDifference = GetAxialDifference(sourceCoords, targetCoords);
            int indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 1)) indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 0)) indexOfTileBase = 1;
            if ((axialDifference.x == 0) && (axialDifference.y == -1)) indexOfTileBase = 2;
            if ((axialDifference.x == 1) && (axialDifference.y == -1)) indexOfTileBase = 3;
            if ((axialDifference.x == 1) && (axialDifference.y == 0)) indexOfTileBase = 4;
            if ((axialDifference.x == 0) && (axialDifference.y == 1)) indexOfTileBase = 5;
            if (_debugLogs) Debug.Log($"shoving in direction " + indexOfTileBase);
            return (_shoveTileHilight[indexOfTileBase]);
        }
        private Vector3Int GetAxialDifference(Vector3Int sourceCoords, Vector3Int targetCoords)
        {
            Vector3Int axialSourceCoords = _mapManager.CastOddRowToAxial(sourceCoords);
            Vector3Int axialTargetCoords = _mapManager.CastOddRowToAxial(targetCoords);
            return axialTargetCoords - axialSourceCoords;
        }
        public void ShoveThisTile(TileNode sourceOfShove, TileNode targetOfShove)
        {
            if (_debugLogs) Debug.Log($"shoving from {sourceOfShove} to " + targetOfShove);
            if ((targetOfShove.Entities == null) || (targetOfShove.Entities.Count == 0)) return;
            Vector3Int shoveDir = GetAxialDifference(sourceOfShove.GridCoordinate, targetOfShove.GridCoordinate);
            List<EntityBase> copiedEntityList = new List<EntityBase>();
            foreach (EntityBase entity in targetOfShove.Entities)
            {
                copiedEntityList.Add(entity);
            }
            foreach (EntityBase entity in copiedEntityList)
            {
                Shovable shovable = entity.GetComponent<Shovable>();
                if (shovable == null) continue;
                shovable.TryShoveDir(shoveDir);
            }
        }
    }
}
