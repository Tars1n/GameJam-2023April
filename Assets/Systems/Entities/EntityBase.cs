using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameJam.Map;

namespace GameJam.Entity
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] protected bool _debugLog = true;
        [SerializeField] protected bool _isShovable = true;
        public bool IsShovable => _isShovable;
        [SerializeField] protected bool _blocksMovement = true;
        public bool BlocksMovement => _blocksMovement;
        [SerializeField] protected TileNode _currentTileNode = null;
        public TileNode CurrentTileNode => _currentTileNode;
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        protected TurnManager _turnManager;
        protected MapManager _mapManager;
        protected MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        protected MapInteractionManager _mapInteractionManager;
        protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected Color _readyState;
        [SerializeField] protected Color _turnOverState;
        [SerializeField] protected bool _hasActionReady;
        public bool HasActionReady => _hasActionReady;
        // [SerializeField] private bool _additionalActions;
        // public bool AdditionalActions {get => _additionalActions; set => _additionalActions = value;}
        // public Action _nextAction;
        [SerializeField] protected bool _isCurrentlyProcessingTurnAction = false;
        public bool IsCurrentlyProcessingTurnAction => _isCurrentlyProcessingTurnAction;
        [SerializeField] protected Color _gizmoColour;
        public Action OnEntitySetup;
        
        
        // public delegate void NextActionDelegate(TileNode tileNode);
        

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            _turnManager = _ref.TurnManager;
            _mapManager = _ref.MapManager;
            _mapInteractionManager = _ref.MapInteractionManager;
            if (_turnManager == null) {Debug.LogWarning($"{this} could not find reference of TurnManager.");}
        }

        public void SetupEntity()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _ref.EntityManager.AddEntity(this);
            LinkToTileNode(null);
            SnapEntityPositionToTile();
            OnEntitySetup?.Invoke();
        }

        public void LinkToTileNode(TileNode tileNode)
        {
            LeaveTileNode();
            _currentTileNode = tileNode;
            if (_currentTileNode == null)
            {
                _currentTileNode = _ref.MapManager.GetTileNodeAtWorldPos(transform.position);
            }
            if (_currentTileNode == null)
            {
                Debug.LogWarning($"{gameObject} failed to link to valid TileNode. Does Object in scene exist outside of valid tiles?");
                return;
            }

            _currentTileNode?.TryAddEntity(this);
        }

        protected void LeaveTileNode()
        {
            if (_currentTileNode == null) 
                {return;}
            _currentTileNode.TryRemoveEntity(this);
            _currentTileNode = null;
        }

        public void ClearTileNode()
        {
            _currentTileNode = null;
        }

        public void SnapEntityPositionToTile()
        {
            if (_currentTileNode != null)
            {
                transform.position = _currentTileNode.WorldPos;
            }
        }

        public virtual void RefreshAction()
        {
            if (this.gameObject.activeInHierarchy == false)
                { return; }
            _hasActionReady = true;
            _spriteRenderer.color = _readyState;

        }

        public abstract void DoTurnAction();

        public virtual void ActionCompleted()
        {
            CompletedTurn();
        }

        public Vector3Int GetAxialPos()
        {
            if (_currentTileNode == null)
            {
                Debug.LogWarning($"Requested Axial Position failed, {this} does not have a valid TileNode.");
                return new Vector3Int(0, 0, 0);
            }
            return _ref.MapManager.CastOddRowToAxial(_currentTileNode.GridCoordinate);
        }

        public virtual void CollidedWithObject()
        {
            _mapInteractionManager.HopEntity(this, this?._currentTileNode, 1);
        }

        protected virtual void CompletedTurn()
        {
            if (_hasActionReady == false)
            {
                //this has already been called by something else, preventing multiple requests to the TurnManager.
                return;
            }

            _hasActionReady = false;
            _spriteRenderer.color = _turnOverState;
            // if (_turnManager.DebugLog) Debug.Log($"{this} has completed it's turn.");
            _turnManager.ActionCompleted();
        }

        public virtual void DoDestroy()
        {
            LeaveTileNode();
            if (_debugLog) { Debug.Log($"{this} is utterly destroyed.");}

            this.gameObject.SetActive(false);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColour;
            Vector3Int tileCoord = Map.Map.WorldToCell(transform.position);
            Vector3 tilePosition = Map.GetWorldPosFromGridCoord(tileCoord);
                
            Gizmos.DrawLine(transform.position, tilePosition);
        }
    }
}
