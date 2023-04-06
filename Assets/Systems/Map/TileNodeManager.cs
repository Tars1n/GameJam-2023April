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
        private int _arrayWidth;
        private int _arrayHeight;
        private TileNode[,] _tileNodesArray;

        public void InitializeTileNodeArray(Tilemap map)
        {   //this is called on Awake from MapManager. Local initialization only.
            _arrayWidth = map.size.x;
            _arrayHeight = map.size.y;
            _tileNodesArray = new TileNode[_arrayWidth, _arrayHeight];

            //TODO set up a way to validate map origin coordinates to ensure they are in-bounds of array.
            
            if (_debugLog)
                Debug.Log($"Created TileNode Array with a total length of: {_tileNodesArray.Length}");
            
            GenerateAllTileNodeEntries(map);
        }

        private void GenerateAllTileNodeEntries(Tilemap map)
        {
            int  tCount = 0;
            for (int x = 0; x < _arrayWidth; x++)
            {
                for (int y = 0; y < _arrayHeight; y++)
                {
                    TileBase mapTile = map.GetTile(new Vector3Int(x, y, 0));
                    if (mapTile == null)
                        continue;
                    TileNode tileNode = new TileNode();
                    _tileNodesArray[x, y] = tileNode;
                    _tileNodesArray[x, y].PreviousStepGridPosition = new Vector3Int(x, y, 0);//set the prev step to itself
                    _tileNodesArray[x, y].GridPosition = new Vector3Int(x, y, 0);
                    //_tileNodesArray[x, y].WorldPos = mapTile.
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

        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            if (!DoesTileNodeExist(coord))
                return new Vector3Int(0,0,-1);
            
            //! this needs to be validated.
            return _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            if (!DoesTileNodeExist(coord))
                return;
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = previousCoord;
        }
        public void SetPreviousStepCoordToItself(Vector3Int coord)
        {
            if (!DoesTileNodeExist(coord))
                return;
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = coord;
        }

        public List<EntityBase> GetEntitiesAtCoord(Vector3Int coord)
        {
            if (!DoesTileNodeExist(coord))
                return null;
            return _tileNodesArray[coord.x, coord.y].Entities;
        }

        public void RemoveEntityAtCoord(Vector3Int coord, EntityBase entity) //TODO prob moved to entity manager
        {
            DoesTileNodeExist(coord);
            if (_tileNodesArray[coord.x, coord.y].Entities.Contains(entity))
            {
                _tileNodesArray[coord.x, coord.y].Entities.Remove(entity);
            }
        }
        public void SetEntityAtCoord(Vector3Int coord, EntityBase entity)
        {
            DoesTileNodeExist(coord);
            _tileNodesArray[coord.x, coord.y].Entities.Add(entity); //TODO prob moved to entity manager
        }

        public bool DoesTileNodeExist(Vector3Int coord)
        {
            TileNode node = GetNodeAtCoord(coord);
            if (node != null)
                return true;
            return false;
        }

        public TileNode GetNodeAtCoord(Vector3Int coord)
        {
            if (AreCoordinatesInBounds(coord))
                return null;
            return _tileNodesArray[coord.x, coord.y];
        }

        private bool AreCoordinatesInBounds(Vector3Int coord)
        {
            if (coord.x < _arrayWidth || coord.x > _arrayWidth || coord.y < _arrayHeight || coord.y > _arrayHeight)
                return false; //coordinates are out of bounds
            return true;
        }
    }
}
