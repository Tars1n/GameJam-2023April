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
        public Vector3Int MoveEntityAlongPathFunc(Vector3Int goalCoord)
        {
            _pathToTake = new Stack<Vector3Int>();    
            StorePath(goalCoord);
            if (_debugLogs)
            {
                Debug.Log($"next step in path " + GetNextStepInPath() + " path length " + _pathToTake.Count);
            }
            return GetNextStepInPath();
        }
        private void StorePath(Vector3Int goalCoord)
        {
            Vector3Int previousCoord = _tileNodeManager.GetPreviousStepCoord(goalCoord);
            if (previousCoord == new Vector3Int(0, 0, -1))
            {
                Debug.LogWarning($"StorePath wanted to add nonexistent TileNode at {goalCoord} to _pathToTake.");
                return;
            }
            if (previousCoord != goalCoord)
            {
                _pathToTake.Push(goalCoord);
                StorePath(previousCoord);
            }            
        }
        private Vector3Int GetNextStepInPath()
        {
            if (_pathToTake.Count == 0)
            {
                Debug.LogWarning("Path to take is empty.");
                return (new Vector3Int(0,0,-1)); //return error
            }
            return _pathToTake.Peek();//would normally use Pop() when following a path.
        }
    }
}
