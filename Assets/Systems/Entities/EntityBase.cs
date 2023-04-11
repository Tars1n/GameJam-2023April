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
        protected MapInteractionManager _mapInteractionManager;
        protected SpriteRenderer _spriteRenderer;
        [SerializeField] private Color _readyState;
        [SerializeField] private Color _turnOverState;
        [SerializeField] private bool _hasActionReady;
        public bool HasActionReady => _hasActionReady;

        protected virtual void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        protected virtual void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _turnManager = _ref.TurnManager;
            _mapInteractionManager = _ref.MapInteractionManager;
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

        public void RefreshAction()
        {
            _hasActionReady = true;
            _spriteRenderer.color = _readyState;
        }

        public abstract void DoTurnAction();

        public virtual void ActionCompleted()
        {
            CompletedTurn();
        }

        protected virtual void CompletedTurn()
        {
            _hasActionReady = false;
            _spriteRenderer.color = _turnOverState;
            if (_turnManager.DebugLog) Debug.Log($"{this} has completed it's turn.");
            _turnManager.ActionCompleted();
        }
    }
}
