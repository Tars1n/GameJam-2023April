using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Pathfinding
{
    public class MoveEntityAlongPath : MonoBehaviour
    {
        private TileClassArrayManager _tileClassArrayManager;
        [SerializeField] private List<Vector3Int> _PathToTake;

        private void Awake()
        {
            _tileClassArrayManager = GetComponent<TileClassArrayManager>();
        }
        public void MoveEntityAlongPathFunc(Vector3Int goalCoord, GameObject entity)
        {
            _PathToTake = new List<Vector3Int>();
            // if (_tileClassArrayManager.GetPreviousStepCoord())
            // _PathToTake.Add()
        }
    }
}
