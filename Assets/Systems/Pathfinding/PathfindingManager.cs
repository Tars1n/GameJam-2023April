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
        [SerializeField] private Tilemap _map;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
        }
        
        public void FillPathInfinate(Vector3Int sourceCoords)
        {
            Vector3Int[] adjacentTiles = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            foreach (Vector3Int tileCoordChecking in adjacentTiles)
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
        private bool IsTileNotExplored(Vector3Int tileCoords)
        {
            if (_tilesExplored.Contains(tileCoords))
            {
                return true;
            }
            return false;
        }
    }
}
