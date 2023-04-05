using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    public class TileNodeManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLog;
        private int _arrayWidth;
        private int _arrayHeight;
        private BoundsInt _mapBounds;
        private TileNode[,] _tileNodesArray;

        public void InitializeTileNodeArray(Tilemap map)
        {   //this is called on Awake from MapManager. Local initialization only.
            _mapBounds = map.cellBounds;
            _tileNodesArray = new TileNode[_mapBounds.xMax - _mapBounds.xMin, _mapBounds.yMax - _mapBounds.yMin];

            //TODO set up a way to validate map origin coordinates to ensure they are in-bounds of array.
            
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
                    TileBase mapTile = map.GetTile(new Vector3Int(x, y, 0));
                    if (mapTile == null)
                        continue;
                    TileNode tileNode = new TileNode();
                    _tileNodesArray[x, y] = tileNode;
                    _tileNodesArray[x, y].PreviousStepGridPosition = new Vector3Int(x, y, 0);//set the prev step to itself
                    _tileNodesArray[x, y].GridPosition = new Vector3Int(x, y, 0);
                    // if (_debugLog) Debug.Log($"entity generated x: { x}, y: {y}, node: { tileNode}");
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
                node.ResetPathingInfo();
            }
        }
        private Vector3Int ConvertCoordsToArrayIndex(Vector3Int coord)
        {
            // if (_debugLog) Debug.Log($"coord converting : {coord}");
            coord.x -= _mapBounds.xMin;
            coord.y -= _mapBounds.yMin;
            // if (_debugLog) Debug.Log($"converted to : {coord}");
            return coord;
        }
        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            coord = ConvertCoordsToArrayIndex(coord);
            if (!DoesTileNodeExistAtArrayIndex(coord))
            {
                Debug.LogError("tile node not found! coord index "+ coord);
                return new Vector3Int(0,0,-1);
            }
            //! this needs to be validated.
            return _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            coord = ConvertCoordsToArrayIndex(coord);
            if (!DoesTileNodeExistAtArrayIndex(coord))
            {
                Debug.LogError($"setting prev step, node does not exist at index {coord}");
                return;
            }
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = previousCoord;
        }
        public void SetPreviousStepCoordToItself(Vector3Int coord)
        {
            Vector3Int arrayIndex = ConvertCoordsToArrayIndex(coord);
            if (!DoesTileNodeExistAtArrayIndex(arrayIndex))
                return;
            _tileNodesArray[arrayIndex.x, arrayIndex.y].PreviousStepGridPosition = coord;
        }
        public List<GameObject> GetEntitiesAtCoord(Vector3Int coord)
        {
            coord = ConvertCoordsToArrayIndex(coord);
            if (!DoesTileNodeExistAtArrayIndex(coord))
                return null;
            return _tileNodesArray[coord.x, coord.y].Entities;
        }

        public void RemoveEntityAtCoord(Vector3Int coord, GameObject entity) //TODO prob moved to entity manager
        {
            coord = ConvertCoordsToArrayIndex(coord);
            DoesTileNodeExistAtArrayIndex(coord);
            if (_tileNodesArray[coord.x, coord.y].Entities.Contains(entity))
            {
                _tileNodesArray[coord.x, coord.y].Entities.Remove(entity);
            }
        }
        public void SetEntityAtCoord(Vector3Int coord, GameObject entity)
        {
            coord = ConvertCoordsToArrayIndex(coord);
            DoesTileNodeExistAtArrayIndex(coord);
            _tileNodesArray[coord.x, coord.y].Entities.Add(entity); //TODO prob moved to entity manager
        }

        public bool DoesTileNodeExistAtArrayIndex(Vector3Int coord)
        {
            TileNode node = GetNodeAtCoord(coord);
            if (node != null)
                return true;
            return false;
        }

        public TileNode GetNodeAtCoord(Vector3Int coord)
        {
            if (!isIndexInBounds(coord)) return null;
            // if (_debugLog) Debug.Log($"returning node at cord x: {coord.x}, y: {coord.y}");
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
