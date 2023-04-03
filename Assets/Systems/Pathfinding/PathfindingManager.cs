using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
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
            // Vector3Int[] adjacentTiles = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            // foreach (Vector3Int tileCoordChecking in adjacentTiles)
            _tilesExplored = new List<Vector3Int>();
            CheckAdjacentTilesToThisTile(sourceCoords);
            if (_tilesInThisStep.Count > 0)
            {
                ConvertNextStepToThis();
                foreach (Vector3Int tileInStep in _tilesInThisStep)
                {
                    CheckAdjacentTilesToThisTile(tileInStep);
                }
            }
        }
        private void CheckAdjacentTilesToThisTile(Vector3Int sourceCoords)
        {

            for (int i = 0; i < 6; i++)
            {
                Vector3Int tileCoordChecking = _mapManager.GetAdjacentHexCoordinate(sourceCoords, i);
                tileCoordChecking += sourceCoords;
                {
                    if (IsTileNotExplored(tileCoordChecking))
                    {
                        if (!_tilesInNextStep.Contains(tileCoordChecking))
                        {
                            _tilesInNextStep.Add(tileCoordChecking);
                        }
                        _tilesExplored.Add(tileCoordChecking);
                    }

                }
            }
        }
        private bool IsTileNotExplored(Vector3Int tileCoords)
        {
            if (!_tilesExplored.Contains(tileCoords))
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
