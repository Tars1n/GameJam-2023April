using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = false;
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

        public void MapAllTileNodesToTarget(Vector3Int targetCoord)
        {
            _tileNodeManager.ResetAllPathing();
            FillPathInfiniteBlockedByObstacles(targetCoord);
            FillPathInfinateNotBlockedByObstacles(targetCoord);

        }
        public void FillPathInfiniteBlockedByObstacles(Vector3Int sourceCoords)
        {            
            //_tileNodeManager.ResetAllPathing();
            FillPathMPBlockedByObstacles(sourceCoords, _infinateFillPathRange);
        }
        public void FillPathInfinateNotBlockedByObstacles(Vector3Int sourceCoords)
        {
            //_tileNodeManager.ResetAllPathing();
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
            if ((!_tilesInNextStep.Contains(coordOfAdjacentTileChecking)) && (IsSafeWalkingTile(coordOfAdjacentTileChecking)))
            {
                _tilesInNextStep.Add(coordOfAdjacentTileChecking);
                //?_overlayTileMap.SetTile(coordOfAdjacentTileChecking, _canMoveOverlay);
                
                TileNode node = _tileNodeManager.GetNodeFromCoords(coordOfAdjacentTileChecking);
                node?.RecordPathing(sourceCoord, false);
            }
        }
        private void CheckCanWalkOnTileNotBlockedByObstacles(Vector3Int coordOfAdjacentTileChecking, Vector3Int sourceCoord)
        {
            if (!_tilesInNextStep.Contains(coordOfAdjacentTileChecking))
            {//tile is added to step even if it is not blocked, it is only hilighted if not blocked.
                _tilesInNextStep.Add(coordOfAdjacentTileChecking);
                
                TileNode node = _tileNodeManager.GetNodeFromCoords(coordOfAdjacentTileChecking);
                node?.RecordPathing(sourceCoord, true);

                //? if (CanWalkOnTile(coordOfAdjacentTileChecking))
                //? {
                //?     _overlayTileMap.SetTile(coordOfAdjacentTileChecking, _canMoveOverlay);
                //? }
            }
        }
        private bool IsSafeWalkingTile(Vector3Int tileCoord)
        {
            TileNode tile = _tileNodeManager.GetNodeFromCoords(tileCoord);
            if (tile == null) { return false; }
            if (tile.IsPitTile)
                { return false; }
            return (CanWalkOnTile(tileCoord));
        }
        private bool CanWalkOnTile(Vector3Int tileCoord)
        {
            TileNode tile = _tileNodeManager.GetNodeFromCoords(tileCoord);
            if (tile == null)    {return false;}
            return _tileNodeManager.GetNodeFromCoords(tileCoord).IsWalkable();
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

        public List<TileNode> GetPathNodes(Vector3Int startingCoord, int range, bool ignoreObstacles)
        {
            List<TileNode> nodeList = new List<TileNode>();
            TileNode node = _tileNodeManager.GetNodeFromCoords(startingCoord);
            for (int i = 0; i < range; i++)
            {
                TileNode nextNode = GetNextNode(node, ignoreObstacles);
                if (node != nextNode || nextNode != null)
                {
                    nodeList.Add(nextNode);
                    node = nextNode;
                }
            }

            return nodeList;
        }

        private TileNode GetNextNode(TileNode node, bool ignoreObstacles)
        {
            if (node == null)   {return null;}
            TileNode nextNode;
            if (ignoreObstacles)
            {
                nextNode = _tileNodeManager.GetNodeFromCoords(node.FlyingPathDirection);
            }
            else
            {
                nextNode = _tileNodeManager.GetNodeFromCoords(node.WalkingPathDirection);
            }

            return nextNode;
        }
    }
}
