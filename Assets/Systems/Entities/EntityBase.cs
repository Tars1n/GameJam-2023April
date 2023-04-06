using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] protected TileNode _currentTileNode;
        public TileNode CurrentTileNode => _currentTileNode;
        protected ReferenceManager _ref;
        protected SpriteRenderer _spriteRenderer;
        public bool HasActedThisRound = false;

        protected virtual void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _ref.EntityManager.AddEntity(this);
            LinkToTileNode();
        }

        public void LinkToTileNode()
        {
            _currentTileNode = _ref.MapManager.GetTileNodeAtWorldPos(transform.position);
            if (_currentTileNode == null)
            {
                Debug.LogWarning($"{gameObject} failed to link to valid TileNode. Does Object in scene exist outside of valid tiles?");
                return;
            }

            _currentTileNode?.TryAddEntity(this);
            SnapEntityPositionToTile();
        }

        public void SnapEntityPositionToTile()
        {
           transform.position = _currentTileNode.WorldPos;   
        }

        public abstract void DoTurnAction();
    }
}
