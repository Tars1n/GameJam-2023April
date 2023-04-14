using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity.Trap
{
    public class TriggerEventManager : MonoBehaviour
    {
        private TileNodeManager _tileNodeManager;
        private ReferenceManager _ref;
        private MapManager _mapManager;
        private MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        [SerializeField] private List<Vector3Int> _triggerLocationTiles;
        //this list creates tiles that the entity can trigger the trap by steppin on.
        private EntityManager _entityManager;

        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _tileNodeManager = _ref.TileNodeManager;
            _entityManager = _ref.EntityManager;
            SetupTriggers();
        }
        public void SetupTriggers()
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
        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            foreach (Vector3Int tilePos in _triggerLocationTiles)
            {
                Vector3 position = Map.GetWorldPosFromGridCoord(tilePos);
                Gizmos.DrawSphere(position, .2f);
            }
        }
        private void ClearTriggerTiles()
        {
            if (_triggerLocationTiles == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tile);
                tileNode.ClearTrigger();
            }
        }
        public virtual void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            //TODO: derive scripts that can customize how they react to this function. Oneshot traps, permanent traps, pressure plates, key to door.
            if (entityBase != null)
            {
                ClearTriggerTiles();
                _entityManager.TryRemoveEntity(entityBase);
                _entityManager.TryRemoveEntity(this.GetComponent<EntityBase>());

            }
        }

        
    }
}
