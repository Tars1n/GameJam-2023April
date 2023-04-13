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
        private MapShoveInteraction _mapShoveInteraction;
        private void Start() 
        {
            _mapManager = GameMaster.Instance.ReferenceManager.MapManager;
            _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
            _mapShoveInteraction = _mapManager.GetComponent<MapShoveInteraction>();
        }
        public void ActivateJumpPushback(TileNode targetNode)
        {
            Vector3Int targetCoord = targetNode.GridCoordinate;
            Vector3Int[] adjacentTileOffsets = _mapManager.GetAllAdjacentHexCoordinates(targetCoord);
            foreach (Vector3Int tileOffset in adjacentTileOffsets)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tileOffset + targetCoord);
                if (tileNode == null) continue;
                if (_mapShoveInteraction.EntityOnThisTileThatCanBeShoved(tileNode))
                {
                    _mapShoveInteraction.ShoveThisTile(targetNode , tileNode);
                }
            }
        }
    }
}
