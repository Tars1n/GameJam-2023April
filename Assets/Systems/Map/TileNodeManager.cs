using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;

namespace GameJam.Map
{
    public class TileNodeManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLog;
        private MapManager _mapManager;
        private int _arrayWidth;
        private int _arrayHeight;
        private BoundsInt _mapBounds;
        private TileNode[,] _tileNodesArray;

        public void InitializeTileNodeArray(Tilemap map)
        {   //this is called on Awake from MapManager. Local initialization only.
            _mapManager = GetComponent<MapManager>();
            _mapBounds = map.cellBounds;
            _tileNodesArray = new TileNode[_mapBounds.xMax - _mapBounds.xMin, _mapBounds.yMax - _mapBounds.yMin];
            
            if (_debugLog)
            {
                Debug.Log($"Created TileNode Array with a total x length of : {_tileNodesArray.GetLength(0)}, y length of : {_tileNodesArray.GetLength(1)}");
                Debug.Log($"Minimum x bounds : {_mapBounds.xMin}, max x bounds : {_mapBounds.xMax}, min y bounds : {_mapBounds.yMin}, max y bounds : {_mapBounds.yMax}");
            }
            GenerateAllTileNodeEntries(map);
        }

        private void GenerateAllTileNodeEntries(Tilemap map)
        {
            int  tCount = 0;
            for (int x = 0; x < _mapBounds.xMax - _mapBounds.xMin; x++)
            {
                for (int y = 0; y < _mapBounds.yMax - _mapBounds.yMin; y++)
                {
                    Vector3Int coord = ConvertArrayIndexToCoords(new Vector3Int(x, y, 0));
                    TileBase mapTile = map.GetTile(coord);
                    if (mapTile == null)
                        continue;
                    TileNode tileNode = new TileNode();
                    _tileNodesArray[x, y] = tileNode;
                    _tileNodesArray[x, y].PreviousStepGridPosition = coord;//set the prev step to itself
                    _tileNodesArray[x, y].GridPosition = coord;
                    _tileNodesArray[x, y].WorldPos = map.CellToWorld(coord);
                    tCount ++;
                }
            }
            if (_debugLog)
                Debug.Log($"TileNodeManager generated {tCount} TileNodes.");
        }

        public void ResetAllPathing()
        {
            foreach (TileNode node in _tileNodesArray)
            {
                node?.ResetPathingInfo();
            }
        }

        public void ClearAllNodeEntities()
        {
            foreach (TileNode node in _tileNodesArray)
            {
                node?.ClearEntities();
            }
        }

        public List<TileNode> GetNeighbourTileNodes(Vector3Int source)
        {//TODO currently it's assumed source is the array index, if source is coords use arrayIndex(line below) instead of source
        // Vector3Int arrayIndex = ConvertCoordsToArrayIndex(source);
            List<TileNode> neighbourNodes = new List<TileNode>();
            Vector3Int[] neighbourCoords = _mapManager.GetAllAdjacentHexCoordinates(source);
            foreach (Vector3Int pos in neighbourCoords)
            {
                if (DoesTileNodeExistAtArrayIndex(pos + source))
                {
                    neighbourNodes.Add(GetNodeAtArrayIndex(pos + source));
                }
            }
            return neighbourNodes;
        }
        private Vector3Int ConvertCoordsToArrayIndex(Vector3Int coord)
        {
            return new Vector3Int(coord.x - _mapBounds.xMin, coord.y - _mapBounds.yMin, coord.z);
        }
        private Vector3Int ConvertArrayIndexToCoords(Vector3Int arrayIndex)
        {
            return new Vector3Int(arrayIndex.x + _mapBounds.xMin, arrayIndex.y + _mapBounds.yMin, arrayIndex.z);
        }
        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            return GetNode(coord).PreviousStepGridPosition;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            GetNode(coord).PreviousStepGridPosition = previousCoord;
        }
        public void SetPreviousStepCoordToItself(Vector3Int coord)
        {
            GetNode(coord).ResetPathingInfo();
        }

        public List<EntityBase> GetEntitiesAtCoord(Vector3Int coord)
        {
            return GetNode(coord).Entities;
        }

        public void RemoveEntityAtCoord(Vector3Int coord, EntityBase entity) //TODO prob moved to entity manager
        {
            TileNode tileNode = GetNode(coord);
            if (tileNode.Entities.Contains(entity))
            {
                tileNode.Entities.Remove(entity);
            }
        }
        public void SetEntityAtCoord(Vector3Int coord, EntityBase entity)
        {
            GetNode(coord).Entities.Add(entity);//TODO prob moved to entity manager
        }
        public TileNode GetNode(Vector3Int coord)
        {
            Vector3Int arrayIndex = ConvertCoordsToArrayIndex(coord);
            if (!DoesTileNodeExistAtArrayIndex(arrayIndex))
                return null;
            return _tileNodesArray[arrayIndex.x, arrayIndex.y];
        }        
        public bool DoesTileNodeExistAtArrayIndex(Vector3Int coord)
        {
            TileNode node = GetNodeAtArrayIndex(coord);
            if (node != null)
                return true;
            Debug.LogError($"node does not exist at index: {coord}");
            return false;
        }

        public TileNode GetNodeAtArrayIndex(Vector3Int coord)
        {
            if (!isIndexInBounds(coord)) return null;
            return _tileNodesArray[coord.x, coord.y];
        }

        private bool isIndexInBounds(Vector3Int arrayIndex)
        {
            if (arrayIndex.x < 0 || arrayIndex.x > _tileNodesArray.GetLength(0) || arrayIndex.y < 0 || arrayIndex.y > _tileNodesArray.GetLength(1))
            {
                Debug.LogError("coords out of array bounds");
                return false; //coordinates are out of bounds
            }
            return true;
        }
    }
}
