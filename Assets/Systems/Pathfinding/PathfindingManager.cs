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
        [SerializeField] private int _infinateFillPathRange = 20;
        private MapManager _mapManager;
        private Tilemap _map;
        private TileNodeManager _tileNodeManager;
        private delegate void CanWalkOnTileDelegate(Vector3Int coordOfAdjacentTileChecking, Vector3Int sourceCoord);

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _map = _mapManager.Map;
        }
        public void FillPathInfiniteBlockedByObstacles(Vector3Int sourceCoords)
        {            
            _tileNodeManager.ResetAllPathing();
            FillPathMPBlockedByObstacles(sourceCoords, _infinateFillPathRange);
        }
        public void FillPathInfinateNotBlockedByObstacles(Vector3Int sourceCoords)
        {
            _tileNodeManager.ResetAllPathing();
            FillPathMPNotBlockedByObstacles(sourceCoords, _infinateFillPathRange);
        }
        
        public void FillPathMPBlockedByObstacles(Vector3Int sourceCoords, int mp)
        {
            CanWalkOnTileDelegate checkCanWalkOnTileDelegate = CheckCanWalkOnTileBlockedByObstacles;//the function that checks tiles that are blocked
            FillPathMP(sourceCoords, mp, checkCanWalkOnTileDelegate);
        }
        public void FillPathMPNotBlockedByObstacles(Vector3Int sourceCoords, int mp)
        {
            CanWalkOnTileDelegate checkCanWalkOnTileDelegate = CheckCanWalkOnTileNotBlockedByObstacles;//the function that checks tiles that are  not blocked
            FillPathMP(sourceCoords, mp, checkCanWalkOnTileDelegate);
        }

        private void FillPathMP(Vector3Int sourceCoords, int mp, CanWalkOnTileDelegate checkCanWalkOnTileDelegate)
        {//checkCanWalkOnTileDelegate is passed so CheckAdjacentTilesToThisTile() knows what function to use for checking blockage
            _tilesInThisStep = new List<Vector3Int>();
            _tilesInNextStep = new List<Vector3Int>();
            _tilesExplored = new List<Vector3Int>();
            _tilesExplored.Add(sourceCoords);
            _tileNodeManager.SetPreviousStepCoordToItself(sourceCoords);
            CheckAdjacentTilesToThisTile(sourceCoords, checkCanWalkOnTileDelegate);
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
                        CheckAdjacentTilesToThisTile(tileInStep, checkCanWalkOnTileDelegate);
                    }
                }
            }
        }

        private void CheckAdjacentTilesToThisTile(Vector3Int sourceCoords, CanWalkOnTileDelegate checkCanWalkOnTileDelegate)
        {
            int index = 0;
            Vector3Int[] tileCoordsCheckingArray = _mapManager.GetAllAdjacentHexCoordinates(sourceCoords);
            foreach (Vector3Int tileCoordInArray in tileCoordsCheckingArray)
            {
                Vector3Int coordOfAdjacentTileChecking = tileCoordInArray + sourceCoords;
                if (IsTileNotExplored(coordOfAdjacentTileChecking))
                {
                    checkCanWalkOnTileDelegate(coordOfAdjacentTileChecking, sourceCoords);//this is where the function to check if the tile is passable is called.
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
        private void CheckCanWalkOnTileNotBlockedByObstacles(Vector3Int coordOfAdjacentTileChecking, Vector3Int sourceCoord)
        {
            if (!_tilesInNextStep.Contains(coordOfAdjacentTileChecking))
            {//tile is added to step even if it is not blocked, it is only hilighted if not blocked.
                _tilesInNextStep.Add(coordOfAdjacentTileChecking);
                _tileNodeManager.SetPreviousStepCoord(coordOfAdjacentTileChecking, sourceCoord);
                if (CanWalkOnTile(coordOfAdjacentTileChecking))
                {
                    _overlayTileMap.SetTile(coordOfAdjacentTileChecking, _canMoveOverlay);
                }
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
