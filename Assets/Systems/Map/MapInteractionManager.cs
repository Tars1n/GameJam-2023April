using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using GameJam.Pathfinding;
using GameJam.Entity;

namespace GameJam.Map
{
    [RequireComponent(typeof(PathfindingManager), typeof(MoveEntityAlongPath))]
    public class MapInteractionManager : MonoBehaviour
    {
        private GameMasterSingleton _gm;
        [SerializeField] private bool _debugLogs = true;
        private MapManager _mapManager;
        private TileNodeManager _tileNodeManager;
        private PathfindingManager _pathfinding;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private Tilemap _map;
        private Tilemap _overlayTilemap;
        private Tilemap _triggerTileMap;
        private Tilemap _mouseMap;
        private Vector3Int _previousTileMousedOver;
        [SerializeField] private TileBase _mouseHoverTileBase;
        [SerializeField] private TileBase _walkPathTileBase;
        [SerializeField] private TileBase _canMoveTileBase;
        [SerializeField] private TileBase _selectionTileBase;
        [SerializeField] private TileBase _activeEntityTileBase;
        [SerializeField] private TileBase _triggerTileHilight;
        [SerializeField] private bool _renderPathToTarget = true;
        [SerializeField] private int _mp = 3;
        [SerializeField] private float slideSpeed = 0.5f;
        [SerializeField] private bool _ignoreObstacles;

        public void Initialize(MapManager mapManager)
        {
            _gm = GameMaster.Instance;
            _map = mapManager.Map;
            _overlayTilemap = mapManager.OverlayMap;
            _triggerTileMap = mapManager.TriggerTilemap;
            _mouseMap = mapManager.MouseInteractionTilemap;
            _mapManager = GetComponent<MapManager>();
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

            if (!GameMaster.Instance.IsPlayerTurn)
                { return; }
            
            //DrawPathFromActiveEntityToMouse();

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
        }

        private void HighlightMouseOverTile(Vector3Int gridCoordinate)
        {
            TileNode tile = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            if (tile == null)
                { return; }

            if (tile.IsSelectable == false)
                { return; }
            
            _mouseMap.SetTile(gridCoordinate, _mouseHoverTileBase);
            RenderPlayerActionTile(tile);
        }

        private void RenderPlayerActionTile(TileNode tile)
        {
            EntityBase activeEntity = GameMaster.Instance.ActiveEntity;
            if (!GameMaster.Instance.IsPlayerTurn || activeEntity == null)
                { return; }
            //check range from active EntityCharacter
            int range = _mapManager.CalculateRange(tile.GridCoordinate, activeEntity.CurrentTileNode.GridCoordinate);
            //if range of 1
            if ( range == 1)
            {
                if (tile.IsWalkable())
                { _mouseMap.SetTile(tile.GridCoordinate, _canMoveTileBase); }
            }
            
            if ( range == 2)
            {
                if (tile.IsWalkable())
                {
                    _mouseMap.SetTile(tile.GridCoordinate, _selectionTileBase);
                }
            }
            //is tile empty and walkable?
        }

        private void DrawPathFromActiveEntityToMouse()
        {
            Entity.EntityBase activeEntity = GameMaster.Instance.ActiveEntity;
            if (activeEntity == null)
                { return; }
            Vector3Int startCoord = activeEntity.CurrentTileNode.GridCoordinate;
            if (startCoord == _previousTileMousedOver)
                { return; }
            
            List<TileNode> pathList = GetPathList(_previousTileMousedOver, activeEntity.CurrentTileNode.GridCoordinate);
            RenderPathTiles(pathList);
            
            HighlightActiveEntityTile();
        }

        private List<TileNode> GetPathList(Vector3Int goal, Vector3Int current)
        {
            _pathfinding.MapAllTileNodesToTarget(goal);
            List<TileNode> pathList = _pathfinding.GetPathNodes(current, _mp, _ignoreObstacles);
            return pathList;
        }

        private void RenderPathTiles(List<TileNode> pathList)
        {
            if (_renderPathToTarget == false)
                { return; }

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

            TryToTakeAction(tileNode);
            RefreshOverlayMap();            
        }

        private void ValidateTileSelection(Vector3Int gridCoordinate, TileNode tileNode)
        {
            if (tileNode == null) { Debug.LogError($"Selected TileNode is null, this should not be possible."); return;}
            if (gridCoordinate != tileNode.GridCoordinate) {Debug.LogError($"Something went wrong: Somehow selected TileNode {tileNode} does not match Grid Coordinates.");}
            
            if (_debugLogs)
            {
                Vector3Int indexPos = _tileNodeManager.ConvertCoordsToArrayIndex(gridCoordinate);
                Debug.Log($"{tileNode.TileType}: Clicked on grid pos {gridCoordinate}. Array position {indexPos}. Entity Count: {tileNode.Entities.Count}.");
                TileNode tile = GameMaster.Instance.ActiveEntity?.CurrentTileNode;
                if (tile != null)
                Debug.Log($"Tile is {_mapManager.CalculateRange(tile.GridCoordinate, gridCoordinate)} range from the Active Entity.");
            }
        }

        private bool SelectedPlayerCharacter(TileNode tileNode)
        {
            bool result = false;
            if (_gm.MultiplePlayerCharacters == false || _gm.IsPlayerTurn == false)
                return false;

            
            EntityCharacter playerCharacter = tileNode.GetPlayerCharacter();
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

        public bool TryToTakeAction(TileNode tile)
        {
            EntityBase entity = GameMaster.Instance.ActiveEntity;
            if (entity == null || !entity.HasActionReady)
                { return false; }

            if (CanMoveToTile(entity, tile, 2))
            {
                MoveEntity(entity, tile);
                entity.CurrentTileNode.TryRemoveEntity(entity);
                entity.LinkToTileNode(tile);
                
                return true;
            }
            return false;
        }

        public bool CanMoveToTile(EntityBase entity, TileNode tile, int range)
        {
            if (tile.IsWalkable() == false)
                { return false; }
            Vector3Int entityPos = entity.CurrentTileNode.GridCoordinate;
            if (_mapManager.CalculateRange(entityPos, tile.GridCoordinate) > range)
                { return false; }
            return true;
        }

        public void MoveEntity(EntityBase entity, TileNode targetTile)
        {
            if (entity?.CurrentTileNode == null || targetTile == null)
                { return; }
            GameObject entityGO = entity.gameObject;
            Vector3 position = targetTile.WorldPos;

            StartCoroutine(LerpObjectToPos(entityGO, position, slideSpeed));

        }

        IEnumerator LerpObjectToPos(GameObject entityGO, Vector3 targetPosition, float duration)
        {
            float timeElapsed = 0;
            Vector3 startPos = entityGO.transform.position;
            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;
                t = t * t * (3f - 2f *t);
                float g = timeElapsed/duration;
                g = 1 - ((1 - g)*(1 - g));


                float x = Mathf.Lerp(startPos.x, targetPosition.x, g);
                float hopHeight = ((startPos.y + targetPosition.y) * 0.5f) +1;
                float launch = Mathf.Lerp(startPos.y, hopHeight, g);
                float land = Mathf.Lerp(hopHeight, targetPosition.y, g);
                float y = Mathf.Lerp(launch, land, t);

                entityGO.transform.position = new Vector3(x, y, 0);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            
            entityGO.transform.position = targetPosition;
            entityGO.GetComponent<EntityBase>().ActionCompleted();
        }
        public void RenderTriggerHilight(Vector3Int tileCoords)
        {
            _triggerTileMap.SetTile(tileCoords, _triggerTileHilight);
        }
        public void ClearTriggerHilight(Vector3Int tileCoords)
        {
            _triggerTileMap.SetTile(tileCoords, null);
        }
    }
    
}
