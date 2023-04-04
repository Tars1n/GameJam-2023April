using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Pathfinding
{
    public class MoveEntityAlongPath : MonoBehaviour
    {
        private bool _deubLogs = true;
        private TileClassArrayManager _tileClassArrayManager;
        [SerializeField] private List<Vector3Int> _PathToTake;

        private void Awake()
        {
            _tileClassArrayManager = GetComponent<TileClassArrayManager>();
        }
        public void MoveEntityAlongPathFunc(Vector3Int goalCoord, GameObject entity)
        {
            _PathToTake = new List<Vector3Int>();   
            _PathToTake.Add(goalCoord);         
            StorePath(goalCoord, entity);
            if (_deubLogs)
            {
                Debug.Log($"next step in path " + GetNextStepInPath());
            }
        }
        private void StorePath(Vector3Int goalCoord, GameObject entity)
        {
            if (_tileClassArrayManager.GetPreviousStepCoord(goalCoord) != goalCoord)
            {
                Vector3Int previousCoord = _tileClassArrayManager.GetPreviousStepCoord(goalCoord);
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
