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
        [SerializeField] private List<Vector3Int> _PathToTake;

        private void Awake()
        {
            _tileNodeManager = GetComponent<TileNodeManager>();
        }
        public void MoveEntityAlongPathFunc(Vector3Int goalCoord, GameObject entity)
        {
            _PathToTake = new List<Vector3Int>();   
            _PathToTake.Add(goalCoord);         
            StorePath(goalCoord, entity);
            if (_debugLogs)
            {
                Debug.Log($"next step in path " + GetNextStepInPath());
            }
        }
        private void StorePath(Vector3Int goalCoord, GameObject entity)
        {
            if (_tileNodeManager.GetPreviousStepCoord(goalCoord) != goalCoord)
            {
                Vector3Int previousCoord = _tileNodeManager.GetPreviousStepCoord(goalCoord);
                _PathToTake.Add(previousCoord);
                StorePath(previousCoord, null);
            }            
        }
        private Vector3Int GetNextStepInPath()
        {
            return _PathToTake[_PathToTake.Count - 2];
        }
    }
}
