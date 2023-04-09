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
        protected TurnManager _turnManager;
        protected SpriteRenderer _spriteRenderer;
        public bool HasActionReady = false;

        protected virtual void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _turnManager = _ref.TurnManager;
            if (_turnManager == null) {Debug.LogWarning($"{this} could not find reference of TurnManager.");}
            _ref.EntityManager.AddEntity(this);
            LinkToTileNode(null);
            SnapEntityPositionToTile();
        }

        public void LinkToTileNode(TileNode tileNode)
        {
            if (tileNode == null)
            {
                _currentTileNode = _ref.MapManager.GetTileNodeAtWorldPos(transform.position);
            }
            else
            {
                _currentTileNode = tileNode;
            }
            if (_currentTileNode == null)
            {
                Debug.LogWarning($"{gameObject} failed to link to valid TileNode. Does Object in scene exist outside of valid tiles?");
                return;
            }

            _currentTileNode?.TryAddEntity(this);
        }

        public void SnapEntityPositionToTile()
        {
           transform.position = _currentTileNode.WorldPos;   
        }

        public abstract void DoTurnAction();

        protected virtual void CompletedTurn()
        {
            HasActionReady = false;
            if (_turnManager.DebugLog) Debug.Log($"{this} has completed it's turn.");
            _turnManager.ActionCompleted();
        }
    }
}
