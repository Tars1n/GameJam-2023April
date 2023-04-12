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
        [SerializeField] private List<Vector3Int> _triggerLocationTiles;
        //this list creates tiles that the entity can trigger the trap by steppin on.
        private EntityManager _entityManager;

        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _tileNodeManager = _ref.TileNodeManager;
            _entityManager = _ref.EntityManager;
            SetUpTrap();
        }
        public void SetUpTrap()
        {
            if (_triggerLocationTiles == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tile);
                tileNode.SetUpTrigger(this);
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
        public void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase != null)
            {
                ClearTriggerTiles();
                _entityManager.TryRemoveEntity(entityBase);
                _entityManager.TryRemoveEntity(GetComponent<EntityBase>());

            }
        }
    }
}
