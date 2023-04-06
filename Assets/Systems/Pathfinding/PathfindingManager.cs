using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        private bool _debugLogs = false;
        [SerializeField] private Tile _tileFloor;
        [SerializeField] private Tile _canMoveOverlay;
        [SerializeField] private Tilemap _overlayTileMap;
        [SerializeField] private List<Vector3Int> _tilesExplored;
        [SerializeField] private List<Vector3Int> _tilesInThisStep;
        [SerializeField] private List<Vector3Int> _tilesInNextStep;
        [SerializeField] private int _fillPathRange = 15;
        private MapManager _mapManager;
        private Tilemap _map;
        private TileNodeManager _tileNodeManager;
        public delegate void CanWalkOnTileDelegate(Vector3Int coordOfAdjacentTileChecking, Vector3Int sourceCoord);
        public CanWalkOnTileDelegate m_canWalkOnTile;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _map = _mapManager.Map;
        }
        public void FillPathInfiniteBlockedByObstacles(Vector3Int sourceCoords)
        {            
            _tileNodeManager.ResetAllPathing();
            FillPathMPBlockedByObstacles(sourceCoords, _fillPathRange);
        }
        
        public void FillPathMPBlockedByObstacles(Vector3Int sourceCoords, int mp)
        {
            CanWalkOnTileDelegate m_canWalkOnTile = CheckCanWalkOnTileBlockedByObstacles;
            FillPathMP(sourceCoords, mp, m_canWalkOnTile);
        }

        private void FillPathMP(Vector3Int sourceCoords, int mp, CanWalkOnTileDelegate m_canWalkOnTile)
        {
            _tilesInThisStep = new List<Vector3Int>();
            _tilesInNextStep = new List<Vector3Int>();
            _tilesExplored = new List<Vector3Int>();
            _tilesExplored.Add(sourceCoords);
            _tileNodeManager.SetPreviousStepCoordToItself(sourceCoords);
            CheckAdjacentTilesToThisTile(sourceCoords, m_canWalkOnTile);
            for (int moveIndex = 1; moveIndex < mp; moveIndex++)
            {
                if (_tilesInNextStep.Count == 0)
                {
                    if (_debugLogs)
                    {
                        Debug.Log($"finished step loop after " + moveIndex + " steps");
                    }
                    return;
                }
                ConvertNextStepToThis();
                if (_tilesInThisStep.Count > 0)
                {
                    foreach (Vector3Int tileInStep in _tilesInThisStep)
                    {
                        CheckAdjacentTilesToThisTile(tileInStep, m_canWalkOnTile);
                    }
                }
            }
        }

        private void CheckAdjacentTilesToThisTile(Vector3Int sourceCoords, CanWalkOnTileDelegate m_canWalkOnTile)
        {
            int index = 0;
            Vector3Int[] tileCoordsCheckingArray = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            foreach (Vector3Int tileCoordInArray in tileCoordsCheckingArray)
            {
                Vector3Int coordOfAdjacentTileChecking = tileCoordInArray + sourceCoords;
                if (IsTileNotExplored(coordOfAdjacentTileChecking))
                {
                    // CheckCanWalkOnTileBlockedByObstacles(coordOfAdjacentTileChecking, sourceCoords);
                    m_canWalkOnTile(coordOfAdjacentTileChecking, sourceCoords);
                    _tilesExplored.Add(coordOfAdjacentTileChecking);
                    index++;
                }
            }
        }

        private void CheckCanWalkOnTileBlockedByObstacles(Vector3Int coordOfAdjacentTileChecking, Vector3Int sourceCoord)
        {
            if ((!_tilesInNextStep.Contains(coordOfAdjacentTileChecking)) && (CanWalkOnTile(coordOfAdjacentTileChecking)))
            {
                _tilesInNextStep.Add(coordOfAdjacentTileChecking);
                _overlayTileMap.SetTile(coordOfAdjacentTileChecking, _canMoveOverlay);
                _tileNodeManager.SetPreviousStepCoord(coordOfAdjacentTileChecking, sourceCoord);
            }
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
