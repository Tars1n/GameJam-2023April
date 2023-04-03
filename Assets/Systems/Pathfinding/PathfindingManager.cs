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
        private MapManager _mapManager;
        private Tilemap _map;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _map = _mapManager.Map;
        }
        
        public void FillPathInfinate(Vector3Int sourceCoords)
        {
            // Vector3Int[] adjacentTiles = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            // foreach (Vector3Int tileCoordChecking in adjacentTiles)
            for (int i =0; i<6; i++)
            {
                Vector3Int tileCoordChecking = _mapManager.GetAdjacentHexCoordinate(sourceCoords, i);
                {
                    if (IsTileNotExplored(tileCoordChecking)) 
                    {
                        if (!_tilesInThisStep.Contains(tileCoordChecking))
                        {
                            _tilesInThisStep.Add(tileCoordChecking);
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
    }
}
