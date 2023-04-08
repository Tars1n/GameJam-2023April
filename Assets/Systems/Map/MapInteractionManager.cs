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
        private Tilemap _mouseMap;
        private Vector3Int _previousTileMousedOver;
        [SerializeField] private TileBase _mouseHoverTileBase;
        [SerializeField] private TileBase _walkPathTileBase;
        [SerializeField] private TileBase _canMoveTileBase;
        [SerializeField] private TileBase _selectionTileBase;
        [SerializeField] private TileBase _activeEntityTileBase;
        [SerializeField] private int _mp = 3;
        [SerializeField] private bool _ignoreObstacles;

        public void Initialize(MapManager mapManager)
        {
            _gm = GameMaster.Instance;
            _map = mapManager.Map;
            _overlayTilemap = mapManager.OverlayMap;
            _mouseMap = mapManager.MouseInteractionTilemap;
            _tileNodeManager = GetComponent<TileNodeManager>();
            _pathfinding = GetComponent<PathfindingManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
        }

        private void Update() {
            if (_gm.TilemapInteractable == false)
                return;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridCoordinate = _map.WorldToCell(mousePosition);

            CheckHighlightedTile(gridCoordinate);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TileBase clickedTile = _map.GetTile(gridCoordinate);
                if (clickedTile == null) {return;}

                OnTileSelected(gridCoordinate);
            }
            _previousTileMousedOver = gridCoordinate;
        }
        
        public void CheckHighlightedTile(Vector3Int gridCoordinate)
        {
            if (_previousTileMousedOver == gridCoordinate) 
                { return; }
            _mouseMap.ClearAllTiles();
            HighlightMouseOverTile(gridCoordinate);
            DrawPathFromActiveEntityToMouse();
        }

        private void HighlightMouseOverTile(Vector3Int gridCoordinate)
        {
            if (IsHighlightableTile(gridCoordinate))
            {
                _mouseMap.SetTile(gridCoordinate, _mouseHoverTileBase);
            }
        }

        private bool IsHighlightableTile(Vector3Int gridCoordinate)
        {
            TileNode node = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            if (node != null)
                { return true; }
            return false;
        }

        private void DrawPathFromActiveEntityToMouse()
        {
            Entity.EntityBase activeEntity = GameMaster.Instance.ActiveEntity;
            if (activeEntity == null)
                { return; }
            Vector3Int startCoord = activeEntity.CurrentTileNode.GridCoordinate;
            if (startCoord == _previousTileMousedOver)
                { return; }
            
            RenderPathTiles(_previousTileMousedOver, activeEntity.CurrentTileNode.GridCoordinate);
            
            HighlightActiveEntityTile();
        }

        private void RenderPathTiles(Vector3Int goal, Vector3Int current)
        {
            _pathfinding.MapAllTileNodesToTarget(goal);
            List<TileNode> pathList = _pathfinding.GetPathNodes(current, _mp, _ignoreObstacles);

            foreach (TileNode node in pathList)
            {
                if (node == null) {return;}
                _mouseMap.SetTile(node.GridCoordinate, _walkPathTileBase);
            }
        }

        public void OnTileSelected(Vector3Int gridCoordinate)
        {
            TileNode tileNode = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            ValidateTileSelection(gridCoordinate, tileNode);

            if (SelectedPlayerCharacter(tileNode))
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

        private bool SelectedPlayerCharacter(TileNode tileNode)
        {
            bool result = false;
            if (_gm.MultiplePlayerCharacters == false || _gm.IsPlayerTurn == false)
                return false;

            
            Entity.EntityCharacter playerCharacter = tileNode.GetPlayerCharacter();
            if (playerCharacter != null)
            {
                _gm.SetActiveEntity(playerCharacter);
                return true;
            }  

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
