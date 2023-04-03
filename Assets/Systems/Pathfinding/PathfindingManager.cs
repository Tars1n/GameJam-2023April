using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField] private Tile _tileFloor;
        [SerializeField] private List<Vector3Int> _tilesExplored;
        [SerializeField] private List<Vector3Int> _tilesInThisStep;
        [SerializeField] private List<Vector3Int> _tilesInNextStep;
        private MapManager _mapManager;
        private Tilemap _map;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _map = _mapManager.Map;
        }
        
        public void FillPathInfinate(Vector3Int sourceCoords)
        {
            _tilesInThisStep = new List<Vector3Int>();
            _tilesInNextStep = new List<Vector3Int>();
            // Vector3Int[] adjacentTiles = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            // foreach (Vector3Int tileCoordChecking in adjacentTiles)
            _tilesExplored = new List<Vector3Int>();
            _tilesExplored.Add(sourceCoords);
            CheckAdjacentTilesToThisTile(sourceCoords);
            ConvertNextStepToThis();
            if (_tilesInThisStep.Count > 0)
            {
                foreach (Vector3Int tileInStep in _tilesInThisStep)
                {
                    CheckAdjacentTilesToThisTile(tileInStep);
                }
            }
        }
        private void CheckAdjacentTilesToThisTile(Vector3Int sourceCoords)
        {
            Vector3Int[] tileCoordsCheckingArray = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            foreach (Vector3Int tileCoordInArray in tileCoordsCheckingArray)
            {
                Vector3Int coordOfAdjacentTileChecking = tileCoordInArray + sourceCoords;
                Debug.Log($"coord of adjacent tile checking" + coordOfAdjacentTileChecking);
            }
            // for (int i = 0; i < 6; i++)
            // {
            //     Vector3Int tileCoordChecking = _mapManager.GetAdjacentHexCoordinate(sourceCoords, i);
            //     tileCoordChecking += sourceCoords;
            //     {
            //         if (IsTileNotExplored(tileCoordChecking))
            //         {
            //             if ((!_tilesInNextStep.Contains(tileCoordChecking)) && (CanWalkOnTile(tileCoordChecking)))
            //             {
            //                 _tilesInNextStep.Add(tileCoordChecking);
            //                 Debug.Log($"i " + i);
            //             }
            //             _tilesExplored.Add(tileCoordChecking);
            //         }

            //     }
            // }
        }
        private bool CanWalkOnTile(Vector3Int tileCoord)
        {
            if (_map.GetTile(tileCoord) == _tileFloor)
            {
                return true;
            }
            return false;
        }
        private bool IsTileNotExplored(Vector3Int tileCoord)
        {
            if (!_tilesExplored.Contains(tileCoord))
            {
                return true;
            }
            return false;
        }
        private void ConvertNextStepToThis()
        {

            _tilesInThisStep = _tilesInNextStep;
            _tilesInNextStep = new List<Vector3Int>();
        }
    }
}
