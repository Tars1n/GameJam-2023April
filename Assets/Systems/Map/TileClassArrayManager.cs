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
        public void RemoveEntityAtCoord(int xCoord, int yCoord, GameObject entity)
        {
            CheckIfClassAtCoord(xCoord, yCoord);
            if (_tileNodesArray[xCoord, yCoord].Entities.Contains(entity))
            {
                _tileNodesArray[xCoord, yCoord].Entities.Remove(entity);
            }
        }
        public void SetEndityAtCoord(int xCoord, int yCoord, GameObject entity)
        {
            CheckIfClassAtCoord(xCoord, yCoord);
            _tileNodesArray[xCoord, yCoord].Entities.Add(entity);
        }
        public List<GameObject> GetEntitiesAtCoord(int xCoord, int yCoord)
        {
            return _tileNodesArray[xCoord, yCoord].Entities;
        }
        public void CheckIfClassAtCoord(int xCoord, int yCoord)
        {
            if (_tileNodesArray[xCoord, yCoord] == null)
            {
                TileNode tileNode = new TileNode();
                _tileNodesArray[xCoord, yCoord] = tileNode;
            }
        }
    }

}
