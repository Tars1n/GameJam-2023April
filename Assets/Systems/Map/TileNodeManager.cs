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
        [SerializeField] private List<TileData> _tileDatas;
        private Dictionary<TileBase, TileData> _dataFromTiles;
        public Dictionary<TileBase, TileData> DataFromTiles => _dataFromTiles;
        

        public void InitializeTileNodeArray(Tilemap map)
        {   
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

        public void DeleteAllTileNodes()
        {
            _tileNodesArray = new TileNode[1,1];
        }

        private void GenerateAllTileNodeEntries(Tilemap map)
        {
            SetupTileData();
            int  tCount = 0;
            for (int x = 0; x <= _mapBounds.xMax - _mapBounds.xMin + 1; x++)
            {
                for (int y = 0; y <= _mapBounds.yMax - _mapBounds.yMin + 1; y++)
                {
                    Vector3Int coord = ConvertArrayIndexToCoords(new Vector3Int(x, y, 0));
                    TileBase mapTile = map.GetTile(coord);
                    if (mapTile == null)
                        continue;
                    TileNode tileNode = new TileNode();
                    tileNode.GridCoordinate = coord;
                    tileNode.WalkingPathDirection = coord;
                    tileNode.FlyingPathDirection = coord;
                    tileNode.WorldPos = map.CellToWorld(coord);
                    tileNode.TileType = mapTile;
                    _tileNodesArray[x, y] = tileNode;
                    tileNode.SetTileData(_dataFromTiles);
                    tCount ++;
                }
            }
            if (_debugLog)
                Debug.Log($"TileNodeManager generated {tCount} TileNodes.");
        }

        private void SetupTileData()
        {
            _dataFromTiles = new Dictionary<TileBase, TileData>();
            foreach (TileData tileData in _tileDatas)
            {
                foreach (TileBase tile in tileData.Tiles)
                {
                    _dataFromTiles.Add(tile, tileData);
                }
            }
        }

        public void RenderAllOcclusionTiles(Tilemap tilemap)
        {
            foreach (TileNode tile in _tileNodesArray)
            {
                if (tile == null) continue;
                if (tile.OcclusionLayer)
                    tilemap.SetTile(tile.GridCoordinate, tile.TileType);
                    Debug.Log($"rendered occlusion tile: {tile.TileType} at {tile.GridCoordinate}");
            }
        }

        public void ResetAllPathing()
        {
            foreach (TileNode node in _tileNodesArray)
            {
                node?.ResetPathingInfo();
            }
        }

        public void ClearAllNodeEntityLists()
        {
            foreach (TileNode node in _tileNodesArray)
            {
                node?.ClearEntityList();
            }
        }

        public List<TileNode> GetNeighbourTileNodes(Vector3Int source)
        {//Assumed source is relative to the array index, if needed use ConvertCoordsToArrayIndex(source);
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
        public Vector3Int ConvertCoordsToArrayIndex(Vector3Int coord)
        {
            return new Vector3Int(coord.x - _mapBounds.xMin, coord.y - _mapBounds.yMin, coord.z);
        }
        public Vector3Int ConvertArrayIndexToCoords(Vector3Int arrayIndex)
        {
            return new Vector3Int(arrayIndex.x + _mapBounds.xMin, arrayIndex.y + _mapBounds.yMin, arrayIndex.z);
        }
        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            // if (RequiredNodeMissing(coord))
            // {
            //     Debug.LogWarning("GetPreviousStepCoord returned an empty TileNode.");
            //      return new Vector3Int(0,0,-1); //functions that use this should check for this error response.
            // }

            //! Is currently open to null reference exceptions. Above validation causes pathfinder to bug out due to how it flags attempting to path a non-existent tile.
            return GetNodeFromCoords(coord).WalkingPathDirection;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            TileNode tileNode = GetNodeFromCoords(coord);
            if (tileNode != null)
            {
                tileNode.WalkingPathDirection = previousCoord;
            }
        }

        public List<EntityBase> GetEntitiesAtCoord(Vector3Int coord)
        {
            return GetNodeFromCoords(coord)?.Entities;
        }

        public int TryRemoveEntityAtCoord(Vector3Int coord, EntityBase entity)
        {
            if (RequiredNodeMissing(coord))
                return 1; //Failed: TileNode does not exist at coordinates.

            TileNode tileNode = GetNodeFromCoords(coord);
            if (tileNode.Entities.Contains(entity) == false)
                return 2; //Failed: Could not remove non-existent Entity from TileNode.
            
            tileNode.Entities.Remove(entity);
            return 0; //Success: Entity removed from TileNode.
        }

        public int TrySetEntityAtCoord(Vector3Int coord, EntityBase entity)
        {
            if (RequiredNodeMissing(coord))
            {
                return 1; //Failed: TileNode does not exist at coordinates.
            }
            bool addEntityResult = GetNodeFromCoords(coord).TryAddEntity(entity);
            if (addEntityResult == false)
                return 2; //Failed: TileNode rejected adding entity.
            return 0; //Success: Entity successfully added to TileNode.
        }

        public TileNode GetNodeFromCoords(Vector3Int coord)
        {
            Vector3Int arrayIndex = ConvertCoordsToArrayIndex(coord);
            if (DoesTileNodeExistAtArrayIndex(arrayIndex) == false)
                return null;
            return _tileNodesArray[arrayIndex.x, arrayIndex.y];
        }

        public TileNode GetTileFromAxial(Vector3Int axialPos)
        {
            Vector3Int coord = _mapManager.CastAxialToOddRow(axialPos);
            return GetNodeFromCoords(coord);
        }

        private bool RequiredNodeMissing(Vector3Int coord)
        {
            if (DoesTileNodeExistAtArrayIndex(coord))
                {return false;}
            //Debug.LogWarning("Tried to perform operations on a non existent TileNode.");
            return true;
        }

        public bool DoesTileNodeExistAtArrayIndex(Vector3Int arrayIndex)
        {
            TileNode node = GetNodeAtArrayIndex(arrayIndex);
            if (node != null)
                return true;
            return false;
        }

        public TileNode GetNodeAtArrayIndex(Vector3Int arrayIndex)
        {
            if (!isIndexInBounds(arrayIndex)) return null;
            return _tileNodesArray[arrayIndex.x, arrayIndex.y];
        }

        private bool isIndexInBounds(Vector3Int arrayIndex)
        {
            if (arrayIndex.x < 0 || arrayIndex.x >= _tileNodesArray.GetLength(0)  || arrayIndex.y < 0 || arrayIndex.y >= _tileNodesArray.GetLength(1) )
            {
                //Debug.LogWarning($"index in array out of bounds {arrayIndex}");
                return false; //coordinates are out of bounds
            }
            return true;
        }
    }
}
