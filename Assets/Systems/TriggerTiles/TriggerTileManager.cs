using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;

namespace GameJam.Map.TriggerTiles
{
    public abstract class TriggerTileManager : MonoBehaviour
    {
        protected TileNodeManager _tileNodeManager;
        protected ReferenceManager _ref;
        [SerializeField] protected Color _gizmoColour = Color.red;
        protected MapManager _mapManager;
        protected MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        [SerializeField] protected List<Vector3Int> _triggerLocationTiles;
        //this list creates tiles that the entity can trigger the trap by steppin on.
        protected EntityManager _entityManager;

        protected virtual void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _tileNodeManager = _ref.TileNodeManager;
            _entityManager = _ref.EntityManager;
            SetupTriggers();
        }
        protected virtual void SetupTriggers()
        {
            if (_triggerLocationTiles == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tile);
                if (tileNode == null)
                {
                    Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tile}");
                    continue;
                }
                tileNode.SetUpTrigger(this);
            }
        }
        protected virtual void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = _gizmoColour;
            foreach (Vector3Int tilePos in _triggerLocationTiles)
            {
                Vector3 position = Map.GetWorldPosFromGridCoord(tilePos);
                Gizmos.DrawSphere(position, .2f);
            }
        }

        protected virtual void OnDestroy()
        {
            ClearTriggerTiles();    
        }
        public void ClearTriggerTiles()
        {
            if (_triggerLocationTiles == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tile);
                tileNode.ClearTrigger();
            }
        }
        public abstract void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode);
    }
}
