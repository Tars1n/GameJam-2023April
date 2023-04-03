using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Pathfinding
{
    public class PathfindingManager : MonoBehaviour
    {
        [SerializeField] private List<Vector3Int> _tilesExplored;
        [SerializeField] private List<Vector3Int> _tilesInThisStep;
        public void FillPathInfinate(Vector3Int sourceCoords)
        {

        }
    }
}
