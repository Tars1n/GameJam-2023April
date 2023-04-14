using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using GameJam.Pathfinding;
using GameJam.Entity;
using GameJam.PlayerInput;
using GameJam.Entity.Abilities;

namespace GameJam.Map
{
    [RequireComponent(typeof(PathfindingManager), typeof(MoveEntityAlongPath), typeof(MirrorManager))]
    public class MapInteractionManager : MonoBehaviour
    {
        private GameMasterSingleton _gm;
        [SerializeField] private bool _debugLogs = true;
        private MapManager _mapManager;
        private TileNodeManager _tileNodeManager;
        private PathfindingManager _pathfinding;
        private MirrorManager _mirrorManager;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private MapShoveInteraction _shoveMapHilights;
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
            _mapManager = GetComponent<MapManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _pathfinding = GetComponent<PathfindingManager>();
            _mirrorManager = GetComponent<MirrorManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
            _shoveMapHilights = GetComponent<MapShoveInteraction>();
            _map = _mapManager.Map;
            _overlayTilemap = _mapManager.OverlayMap;
            _triggerTileMap = _mapManager.TriggerTilemap;
            _mouseMap = _mapManager.MouseInteractionTilemap;
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
            if (_mirrorManager.IsReflecting())
            {

            }
        }

        private void HighlightMouseOverTile(Vector3Int gridCoordinate)
        {
            TileNode tile = _tileNodeManager.GetNodeFromCoords(gridCoordinate);
            if (tile == null)
                { return; }

            if (tile.IsSelectable == false)
                { return; }
            
            _mouseMap.SetTile(gridCoordinate, _mouseHoverTileBase);
            RenderPlayerActionTile(null, tile);
            _mirrorManager.RenderMirroredSelection(_gm.ActiveEntity, tile);
            
        }

        public void RenderPlayerActionTile(EntityBase entity, TileNode tile)
        {
            if (entity == null)
                { entity = GameMaster.Instance.ActiveEntity; }
            if (!GameMaster.Instance.IsPlayerTurn || entity == null)
                { return; }
            //check range from active EntityCharacter
            int range = _mapManager.CalculateRange(tile.GridCoordinate, entity.CurrentTileNode.GridCoordinate);
            if ( range == 1)
            {
                if (tile.IsWalkable())
                {
                     _mouseMap.SetTile(tile.GridCoordinate, _canMoveTileBase); 
                    return;
                }
                _shoveMapHilights.TryRenderShoveHilight(entity.CurrentTileNode, tile);
            }
            if ( range == 2)
            {
                if (tile.IsWalkable())
                {
                    _mouseMap.SetTile(tile.GridCoordinate, _selectionTileBase);
                }
            }
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

            //mirror player input
            //get origin and action Coordinates
            //send mirrored version of that interaction to mirrored chars.
            EntityBase entity = GameMaster.Instance.ActiveEntity;
            _mirrorManager.TryMirrorEntityAction(entity, tileNode);

            TryToTakeAction(null, tileNode);
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
                if (tileNode.Entities.Count > 0)
                {   
                    foreach (EntityBase entity in tileNode.Entities)
                    {
                        Debug.Log($"  - {entity}");
                    }
                }
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

        public bool TryToTakeAction(EntityBase entity, TileNode tile)
        {
            if (entity == null)
                { entity = GameMaster.Instance.ActiveEntity; }
            if (entity == null || !entity.HasActionReady)
                { return false; }
            if (CanShoveTile(entity, tile))
            {                
                _shoveMapHilights.ShoveThisTile(entity.CurrentTileNode, tile);
                return true;
            }
            if (CanMoveToTile(entity, tile, 1))
            {
                MoveEntityUpdateTileNodes(entity, tile);
                return true;
            }
            if ((entity.GetComponent<JumpAndShove>() != null) && (CanMoveToTile(entity, tile, 2)))
            {                //jump and shove
                // entity.AdditionalActions = true;
                entity.GetComponent<JumpAndShove>().SubscribeToEntityActionCompleted(tile);
                MoveEntityUpdateTileNodes(entity, tile);
                // entity.GetComponent<JumpAndShove>().ActivateJumpPushback(tile);
                
            }
            return false;
        }
        public void MoveEntityUpdateTileNodes(EntityBase entity, TileNode tile)
        {
            MoveEntity(entity, tile);
            // entity.CurrentTileNode.TryRemoveEntity(entity);
            // entity.LinkToTileNode(tile);
        }

        public bool CanShoveTile(EntityBase entity, TileNode tile)
        {
            Vector3Int entityPos = entity.CurrentTileNode.GridCoordinate;
            if ((_shoveMapHilights.EntityOnThisTileThatCanBeShoved(tile)) && (_mapManager.CalculateRange(entityPos, tile.GridCoordinate) == 1))
            {
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

            bool removed = entity.CurrentTileNode.TryRemoveEntity(entity);
            if (!removed) { Debug.LogWarning($"{entity.CurrentTileNode} failed to remove {entity}.");}
            StartCoroutine(DoHopEntityToPos(entityGO, position, slideSpeed));
        }

        IEnumerator DoHopEntityToPos(GameObject entityGO, Vector3 targetPosition, float duration)
        {
            float timeElapsed = 0;
            Vector3 startPos = entityGO.transform.position;
            while (timeElapsed < duration)
            {
                if (entityGO == null)
                    { yield break; }

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
            EntityBase entity = entityGO.GetComponent<EntityBase>();
            entity.LinkToTileNode(null);
            entity.ActionCompleted();
        }

        public void ShoveEntity(EntityBase entity, TileNode targetTile)
        {
            if (entity?.CurrentTileNode == null || targetTile == null)
                { return; }
            GameObject entityGO = entity.gameObject;
            Vector3 position = targetTile.WorldPos;
            
            bool removed = entity.CurrentTileNode.TryRemoveEntity(entity);
            if (!removed) { Debug.LogWarning($"{entity.CurrentTileNode} failed to remove {entity}.");}
            StartCoroutine(DoShoveEntityToPos(entityGO, position, slideSpeed));
        }

        IEnumerator DoShoveEntityToPos(GameObject entityGO, Vector3 targetPosition, float duration)
        {
            float timeElapsed = 0;
            Vector3 startPos = entityGO.transform.position;
            while (timeElapsed < duration)
            {
                // float t = timeElapsed / duration;
                // t = t * t * (3f - 2f *t);
                float g = timeElapsed/duration;
                g = 1 - ((1 - g)*(1 - g));


                float x = Mathf.Lerp(startPos.x, targetPosition.x, g);
                float y = Mathf.Lerp(startPos.y, targetPosition.y, g);

                entityGO.transform.position = new Vector3(x, y, 0);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            entityGO.transform.position = targetPosition; 
            EntityBase entity = entityGO.GetComponent<EntityBase>();
            entity.LinkToTileNode(null);
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
