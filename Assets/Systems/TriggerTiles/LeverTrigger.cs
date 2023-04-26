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
        public bool ActivatedThisTurn = false;
        [SerializeField] private List<Vector3Int> _tilesToToggle = new List<Vector3Int>();
        [SerializeField] private TileBase _unpulledTileState;
        [SerializeField] private TileBase _pulledTileState;
        [SerializeField] private bool _toggleAnyTileState = true;
        private TileBase _currentTileState;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        //This sets the current world position of the Lever Entity to be the trigger tile itself.
        private void OnValidate()
        {
            _triggerLocationTiles.Clear();
            if (_mapManager == null) { return; }
            Vector3Int coordinate = Map.Map.WorldToCell(this.transform.position);
            _triggerLocationTiles.Add(coordinate);
            
        }

        public override void SetupTriggerTiles()
        {
            base.SetupTriggerTiles();
            RenderToggleTiles();
        }

        private void RenderToggleTiles()
        {
            foreach (Vector3Int coord in _tilesToToggle)
            {
                _mapManager.TriggerTilemap.SetTile(coord, _triggerTile);
            }
        }


        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            ToggleLever();
        }

        public void ToggleLever()
        {
            if (ActivatedThisTurn)
                { return; }
            ActivatedThisTurn = true;
            _leverPulled = !_leverPulled;
            _animator?.SetBool("LeverPulled", _leverPulled);
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.LeverToggled);
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.TilesAltered);

            if (_leverPulled)
                { _currentTileState = _pulledTileState; }
            else
                { _currentTileState = _unpulledTileState; }

            DoToggleTiles();
            _mapManager.RenderOcclusionTiles();
        }

        private void DoToggleTiles()
        {
            foreach (Vector3Int tileCoord in _tilesToToggle)
            {
                TileNode tile = _ref.TileNodeManager.GetNodeFromCoords(tileCoord);
                if (tile == null) { continue; }
                
                if (_toggleAnyTileState)
                {
                    ToggleTile(tile);
                }
                else
                {
                    SetToSpecificTile(tile);
                }

                tile.SetTileData(_ref.TileNodeManager.DataFromTiles);
                tile.CollidedWith();
            }
        }

        private void ToggleTile(TileNode tile)
        {
            if (tile.TileType != _pulledTileState)
            {
                tile.TileType = _pulledTileState;
                _mapManager.Map.SetTile(tile.GridCoordinate, _pulledTileState);
                return;
            }

            tile.TileType = _unpulledTileState;
            _mapManager.Map.SetTile(tile.GridCoordinate, _unpulledTileState);
        }

        private void SetToSpecificTile(TileNode tile)
        {
            tile.TileType = _currentTileState;
            _mapManager.Map.SetTile(tile.GridCoordinate, _currentTileState);
        }

        protected override void OnDrawGizmos()
        {
            if (Map == null) return;
            Gizmos.color = _gizmoColour;
            foreach (Vector3Int tilePos in _triggerLocationTiles)
            {
                Vector3 position = _mapManager.GetWorldPosFromGridCoord(tilePos);
                Gizmos.DrawSphere(position, .2f);
            }
            foreach (Vector3Int tilePos in _tilesToToggle)
            {
                Vector3 position = _mapManager.GetWorldPosFromGridCoord(tilePos);
                Gizmos.DrawCube(position, new Vector3(0.3f, 0.2f, 0.2f));
            }
        }
    }
}
