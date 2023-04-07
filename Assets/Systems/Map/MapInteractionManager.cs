using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using GameJam.Pathfinding;

namespace GameJam.Map
{
    [RequireComponent(typeof(PathfindingManager), typeof(MoveEntityAlongPath))]
    public class MapInteractionManager : MonoBehaviour
    {
        private GameMasterSingleton _gm;
        [SerializeField] private bool _debugLogs = true;
        private TileNodeManager _tileNodeManager;
        private PathfindingManager _pathfinding;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private Tilemap _map;
        private Tilemap _overlayTilemap;
        [SerializeField] private TileBase _canMoveTileBase;
        [SerializeField] private TileBase _selectionTileBase;
        [SerializeField] private TileBase _activeEntityTileBase;
        [SerializeField] private int _mp = 3;

        public void Initialize(MapManager mapManager)
        {
            _gm = GameMaster.Instance;
            _map = mapManager.Map;
            _overlayTilemap = mapManager.OverlayMap;
            _tileNodeManager = GetComponent<TileNodeManager>();
            _pathfinding = GetComponent<PathfindingManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
        }

        private void Update() {
            if (_gm.TilemapInteractable == false)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3Int gridCoordinate = _map.WorldToCell(mousePosition);

                TileBase clickedTile = _map.GetTile(gridCoordinate);
                if (clickedTile == null) {return;}

                OnTileSelected(gridCoordinate);
            }
        }
        public void OnTileSelected(Vector3Int gridCoordinate)
        {
            TileNode tileNode = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            ValidateTileSelection(gridCoordinate, tileNode);

            if (SelectedPlayerCharacter(gridCoordinate))
            {
                RefreshOverlayMap();
                return;
            }
            TileBase selectedTile = _overlayTilemap.GetTile(gridCoordinate);
            if (selectedTile == _canMoveTileBase)
            {
                _moveEntityAlongAPath.MoveEntityAlongPathFunc(gridCoordinate);
                RefreshOverlayMap();
                return;
            }
            if (selectedTile) //? does this work?
            {
                //tile already selected, deselecting
                RefreshOverlayMap();
                return;
            }

            _overlayTilemap.ClearAllTiles();
            _overlayTilemap.SetTile(gridCoordinate, _selectionTileBase);
            _pathfinding.FillPathMPNotBlockedByObstacles(gridCoordinate, _mp);
            HighlightActiveEntityTile();
        }

        private void ValidateTileSelection(Vector3Int gridCoordinate, TileNode tileNode)
        {
            if (tileNode == null) { Debug.LogError($"Selected TileNode is null, this should not be possible."); return;}
            if (gridCoordinate != tileNode.GridCoordinate) {Debug.LogError($"Something went wrong: Somehow selected TileNode {tileNode} does not match Grid Coordinates.");}
            
            if (_debugLogs)
            {
                Vector3Int indexPos = _tileNodeManager.ConvertCoordsToArrayIndex(gridCoordinate);
                Debug.Log($"{tileNode.TileType}: Clicked on grid pos {gridCoordinate}. Array position {indexPos}. Entity Count: {tileNode.Entities.Count}");
            }
        }

        private bool SelectedPlayerCharacter(Vector3Int gridCoordinate)
        {
            bool result = false;
            if (_gm.MultiplePlayerCharacters == false || _gm.IsPlayerTurn == false)
                return false;

            TileNode tileNode = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            if (tileNode != null)
            {}

            return result;
        }
        public void RefreshOverlayMap()
        {
            _overlayTilemap.ClearAllTiles();
            HighlightActiveEntityTile();
        }

        private void HighlightActiveEntityTile()
        {
            Entity.EntityBase entity = _gm.ActiveEntity;
            TileNode tile = entity?.CurrentTileNode;
            if (tile != null)
            {
                _overlayTilemap.SetTile(tile.GridCoordinate, _activeEntityTileBase);
            }
        }
    }
    
}
