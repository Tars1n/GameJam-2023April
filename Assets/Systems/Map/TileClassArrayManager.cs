using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Map
{
    public class TileClassArrayManager : MonoBehaviour
    {
        private const int _tileXMax = 15;
        private const int _tileYMax = 15;
        private TileNode[,] _tileNodesArray;        

        private void Awake()
        {
            _tileNodesArray = new TileNode[_tileXMax, _tileYMax];            
        }
        public Vector3Int GetPreviousStepCoord(Vector3Int coord)
        {
            CheckIfClassAtCoord(coord);
            return _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition;
        }
        public void SetPreviousStepCoord(Vector3Int coord, Vector3Int previousCoord)
        {
            CheckIfClassAtCoord(coord);
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = previousCoord;
        }
        public void SetPreviousStepCoordToItself(Vector3Int coord)
        {
            CheckIfClassAtCoord(coord);
            _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = coord;
        }
        public void RemoveEntityAtCoord(Vector3Int coord, GameObject entity)
        {
            CheckIfClassAtCoord(coord);
            if (_tileNodesArray[coord.x, coord.y].Entities.Contains(entity))
            {
                _tileNodesArray[coord.x, coord.y].Entities.Remove(entity);
            }
        }
        public void SetEndityAtCoord(Vector3Int coord, GameObject entity)
        {
            CheckIfClassAtCoord(coord);
            _tileNodesArray[coord.x, coord.y].Entities.Add(entity);
        }
        public List<GameObject> GetEntitiesAtCoord(Vector3Int coord, int yCoord)
        {
            return _tileNodesArray[coord.x, coord.y].Entities;
        }
        public void CheckIfClassAtCoord(Vector3Int coord)
        {
            if (_tileNodesArray[coord.x, coord.y] == null)
            {
                TileNode tileNode = new TileNode();
                _tileNodesArray[coord.x, coord.y] = tileNode;
                _tileNodesArray[coord.x, coord.y].PreviousStepGridPosition = coord;//set the prev step to itself
                _tileNodesArray[coord.x, coord.y].GridPosition = coord;
            }
        }
    }

}
