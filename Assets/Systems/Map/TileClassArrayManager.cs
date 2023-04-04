using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    public class TileClassArrayManager : MonoBehaviour
    {
        private bool _debugLog = true;
        private const int _tileXMax = 15;
        private const int _tileYMax = 15;
        private TileNode[,] _tileNodesArray;   
        private MapManager _mapManager; 
        private Tilemap _tileMap;

        private void Awake()
        {  
            _mapManager = GetComponent<MapManager>();    
            _tileMap = _mapManager.Map;    
        }
        private void Start()
        {   
            SetTileArray();
        }
        private void SetTileArray()
        {
            BoundsInt bounds = _tileMap.cellBounds;
            _tileNodesArray = new TileNode[bounds.max.x , bounds.max.y];
            if (_debugLog) Debug.Log($"bounds max.x " + bounds.max.x);            
            for (int x = bounds.min.x; x < bounds.max.x; x ++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y ++)
                {
                    Vector3Int coords = new Vector3Int(x, y, 0);
                    TileBase tileBase = _tileMap.GetTile(coords);
                    if ((tileBase == null) || (!tileBase))
                    {
                        continue;
                    }    
                    Debug.Log($"coords " + coords);
                    CheckIfClassAtCoordAndCreate(coords);
                }
            }
            if (_debugLog) Debug.Log($"tile nodes array " + _tileNodesArray);
        }
        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            return _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = previousCoord;
        }
        public void SetPreviousStepCoordToItself(Vector3Int coord)
        {
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = coord;
        }
        public void RemoveEntityAtCoord(Vector3Int coord, GameObject entity)
        {
            if (_tileNodesArray[coord.x, coord.y].Entities.Contains(entity))
            {
                _tileNodesArray[coord.x, coord.y].Entities.Remove(entity);
            }
        }
        public void SetEndityAtCoord(Vector3Int coord, GameObject entity)
        {
            _tileNodesArray[coord.x, coord.y].Entities.Add(entity);
        }
        public List<GameObject> GetEntitiesAtCoord(Vector3Int coord, int yCoord)
        {
            return _tileNodesArray[coord.x, coord.y].Entities;
        }
        public void CheckIfClassAtCoordAndCreate(Vector3Int coord)
        {
                TileNode tileNode = new TileNode();
                _tileNodesArray[coord.x, coord.y] = tileNode;
                _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = coord;//set the prev step to itself
                _tileNodesArray[coord.x, coord.y].GridPosition = coord;
        }
    }

}
