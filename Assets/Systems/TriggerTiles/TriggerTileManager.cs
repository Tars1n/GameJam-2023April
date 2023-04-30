using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;
using UnityEngine.Tilemaps;
using GameJam.Dialogue;

namespace GameJam.Map.TriggerTiles
{
    public abstract class TriggerTileManager : MonoBehaviour
    {
        [SerializeField] protected MapManager _mapManager;
        [SerializeField] protected TileBase _triggerTile;
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] protected Color _colour = Color.white;
        protected MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        [SerializeField] protected List<Vector3Int> _triggerLocationTiles;
        [SerializeReference] protected List<DialoguePieceClass> _triggeredDialogue;
        protected bool _dialogueUnread = true;

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
                Tile t = (Tile)_triggerTile;
                tileNode.SetUpTrigger(this, _triggerTile, t.color);
            }
        }
        protected virtual void OnDrawGizmos()
        {
            if (Map == null) { return; }
            // Draw a yellow sphere at the transform's position
            Gizmos.color = _colour;
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
        public virtual void ClearTriggerTiles()
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

        protected void TryTriggerDialogue()
        {
            if (_triggeredDialogue != null && _triggeredDialogue.Count > 0 && _dialogueUnread)
            {
                _dialogueUnread = false;
                _ref.DialogueManager.DoDialogue(_triggeredDialogue);
            }
        }
    }
}
