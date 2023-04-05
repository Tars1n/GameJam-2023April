using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileNode
    {
        public TileBase TileType;
        public Vector3Int GridPosition;
        public Vector3Int PreviousStepGridPosition;
        public List<GameObject> Entities;

        public void ResetPathingInfo()
        {
            PreviousStepGridPosition = GridPosition;
        }
    }
}
