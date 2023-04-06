using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] private TileNode _currentTileNode;
        private ReferenceManager _ref;
        protected SpriteRenderer _spriteRenderer;
        public bool HasActedThisRound = false;

        protected virtual void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _ref.EntityManager.AddEntity(this);
            LinkToTileNode();
        }

        private void LinkToTileNode()
        {
            _currentTileNode = _ref.MapManager.GetTileNodeAtWorldPos(transform.position);
            _currentTileNode?.AddEntity(this);
        }

        public void SnapToTile()
        {
           // Vector3 pos = _ref.MapManager.
            
        }

        public abstract void DoTurnAction();
    }
}
