using System.Collections;
using System.Collections.Generic;
using GameJam.Entity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam.Map.TriggerTiles
{
    public class LeverTrigger : TriggerTileManager
    {
        [SerializeField] private bool _leverPulled = false;
        [SerializeField] private List<Vector3Int> _tilesToToggle = new List<Vector3Int>();
        [SerializeField] private TileBase _unpulledTileState;
        [SerializeField] private TileBase _pulledTileState;
        private TileBase _currentTileState;
        private Tilemap _map;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            if (_triggerLocationTiles.Count != 0)
            {
                _triggerLocationTiles.Clear();
            }
        }

        //This sets the current world position of the Lever Entity to be the trigger tile itself.
        protected override void Start()
        {
            Vector3Int coordinate = Map.Map.WorldToCell(this.transform.position);
            _triggerLocationTiles.Add(coordinate);
            _map = _mapManager.Map;
            base.Start();
        }

        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            ToggleLever();
        }

        public void ToggleLever()
        {
            _leverPulled = !_leverPulled;
            _animator?.SetBool("LeverPulled", _leverPulled);

            if (_leverPulled)
                { _currentTileState = _pulledTileState; }
            else
                { _currentTileState = _unpulledTileState; }

            DoToggleTiles();
        }

        private void DoToggleTiles()
        {
            foreach (Vector3Int tileCoord in _tilesToToggle)
            {
                TileNode tile = _tileNodeManager.GetNodeFromCoords(tileCoord);
                // if (tile == null || tile?.Entities.Count > 0) { continue; }
                
                tile.TileType = _currentTileState;
                _map.SetTile(tileCoord, _currentTileState);

                tile.SetTileData(_tileNodeManager.DataFromTiles);
            }
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColour;
            foreach (Vector3Int tilePos in _tilesToToggle)
            {
                Vector3 position = Map.GetWorldPosFromGridCoord(tilePos);
                Gizmos.DrawCube(position, new Vector3(0.3f, 0.2f, 0.2f));
            }
        }
    }
}
