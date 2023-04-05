using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Pathfinding
{
    public class MoveEntityAlongPath : MonoBehaviour
    {
        private bool _debugLogs = true;
        private TileNodeManager _tileNodeManager;
        [SerializeField] private Stack<Vector3Int> _pathToTake;

        private void Awake()
        {
            _tileNodeManager = GetComponent<TileNodeManager>();
        }
        public void MoveEntityAlongPathFunc(Vector3Int goalCoord, GameObject entity)
        {
            _pathToTake = new Stack<Vector3Int>();    
            StorePath(goalCoord, entity);
            if (_debugLogs)
            {
                Debug.Log($"next step in path " + GetNextStepInPath() + " path length " + _pathToTake.Count);
            }
        }
        private void StorePath(Vector3Int goalCoord, GameObject entity)
        {
            if (_tileNodeManager.GetPreviousStepCoord(goalCoord) != goalCoord)
            {
                Vector3Int previousCoord = _tileNodeManager.GetPreviousStepCoord(goalCoord);
                _pathToTake.Push(goalCoord);
                StorePath(previousCoord, null);
            }            
        }
        private Vector3Int GetNextStepInPath()
        {
            return _pathToTake.Peek();//would normally use Pop() when following a path.
        }
    }
}
