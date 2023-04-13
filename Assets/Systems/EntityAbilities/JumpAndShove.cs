using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity.Abilities
{
    public class JumpAndShove : MonoBehaviour
    {
        private MapManager _mapManager;
        private TileNodeManager _tileNodeManager;
        
        private void Start() 
        {
            _mapManager = GameMaster.Instance.ReferenceManager.MapManager;
            _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
        }
        public void ActivateJumpPushback(TileNode targetNode)
        {
            Vector3Int[] adjacentTileCoords = _mapManager.GetAllAdjacentHexCoordinates(targetNode.GridCoordinate)
            foreach (Vector3Int tileCoord in adjacentTileCoords)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tileCoord);

            }
        }
    }
}
