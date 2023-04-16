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
        private EntityBase _entityBase;

        private void Awake()
        {
            _entityBase = GetComponent<EntityBase>();
        }
        private void Start() 
        {
            _mapManager = GameMaster.Instance.ReferenceManager.MapManager;
            _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
            _mapShoveInteraction = _mapManager.GetComponent<MapShoveInteraction>();
        }
        // public void SubscribeToEntityActionCompleted(TileNode targetNode)
        // {
        //     _entityBase._nextAction += ActivateJumpPushback;
        // }
        // private void OnEnable()
        // {
        //     _entityBase._nextAction -= ActivateJumpPushback;
        // }
        public void ActivateJumpPushback()
        {
            Debug.LogWarning("ActivateJumpPushback called!");
            TileNode targetNode = _entityBase.CurrentTileNode;
            if (targetNode == null)
            {
                Debug.LogWarning($"Jump and Shove failed because {_entityBase} has no CurrentTileNode.");
                return;
            }
            Vector3Int targetCoord = targetNode.GridCoordinate;
            Vector3Int[] adjacentTileOffsets = _mapManager.GetAllAdjacentHexCoordinates(targetCoord);
            foreach (Vector3Int tileOffset in adjacentTileOffsets)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tileOffset + targetCoord);
                if (tileNode == null) continue;
                if (_mapShoveInteraction.EntityOnThisTileThatCanBeShoved(tileNode))
                {
                    _mapShoveInteraction.ShoveThisTile(targetNode , tileNode, 1);
                }
            }
            // _entityBase._nextAction -= ActivateJumpPushback;
        }
    }
}
