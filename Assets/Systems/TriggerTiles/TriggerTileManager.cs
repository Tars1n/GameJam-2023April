using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.Map.TriggerTiles
{
    public abstract class TriggerTileManager : MonoBehaviour
    {
        [SerializeField] protected MapManager _mapManager;
        [SerializeField] private TileBase _triggerTile;
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] protected Color _gizmoColour = Color.red;
        protected MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        [SerializeField] protected List<Vector3Int> _triggerLocationTiles;
        //this list creates tiles that the entity can trigger the trap by steppin on.

        protected virtual void Start()
        {
            _mapManager = _ref.MapManager;
        }
        public virtual void SetupTriggerTiles()
        {
            if (_ref.TileNodeManager == null)
            {
                Debug.LogWarning($"SetupTriggerTiles did not have a reference to TileNodeManager.");
                return;
            }
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _ref.TileNodeManager.GetNodeFromCoords(tile);
                if (tileNode == null)
                {
                    Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tile}");
                    continue;
                }
                tileNode.SetUpTrigger(this, _triggerTile);
            }
        }
        protected virtual void OnDrawGizmos()
        {
            if (Map == null) { return; }
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
            // ClearTriggerTiles();    
        }
        public void ClearTriggerTiles()
        {
            if (_triggerLocationTiles == null || _ref.TileNodeManager == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _ref.TileNodeManager?.GetNodeFromCoords(tile);
                if (tileNode != null)
                    tileNode.ClearTrigger();
            }
        }
        public abstract void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode);
    }
}
