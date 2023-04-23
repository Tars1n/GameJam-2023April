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
        private MapShoveInteraction _shoveInteraction;
        public MapShoveInteraction MapShoveInteraction => _shoveInteraction;
        private TurnManager _turnManager;
        private Tilemap _map;
        private Tilemap _overlayTilemap;
        private Tilemap _triggerTileMap;
        private Tilemap _mouseMap;
        private Vector3Int _previousTileMousedOver;
        [SerializeField] private TileBase _mouseHoverTileBase;
        [SerializeField] private TileBase _walkPathTileBase;
        [SerializeField] private TileBase _canMoveTileBase;
        [SerializeField] private TileBase _selectionTileBase;
        [SerializeField] private TileBase _monsterTelegraphTileBase;
        public TileBase MonsterTelegraph => _monsterTelegraphTileBase;
        [SerializeField] private TileBase _activeEntityTileBase;
        // [SerializeField] private TileBase _triggerTileHilight;
        [SerializeField] private bool _renderPathToTarget = true;
        [SerializeField] private int _mp = 3;
        [SerializeField] private float _slideSpeed = 0.5f;
        [SerializeField] private AudioClip _tileMouseover;
        [SerializeField] private bool _ignoreObstacles;

        public void Initialize(MapManager mapManager)
        {
            _gm = GameMaster.Instance;
            _mapManager = GetComponent<MapManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _pathfinding = GetComponent<PathfindingManager>();
            _mirrorManager = GetComponent<MirrorManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
            _shoveInteraction = GetComponent<MapShoveInteraction>();
            _map = _mapManager.Map;
            _overlayTilemap = _mapManager.OverlayMap;
            _triggerTileMap = _mapManager.TriggerTilemap;
            _mouseMap = _mapManager.MouseInteractionTilemap;
            _turnManager = _gm.ReferenceManager.TurnManager;
            _turnManager.OnPlayerTurnBegins += DirtyMousePosition;
        }

        private void Update() {
            if (_gm.TilemapInteractable == false)
                return;
            if (GameMaster.GetSingleton().GameSuspended == true)
            {
                // RefreshOverlayMap();
                return;
            }

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int gridCoordinate = _map.WorldToCell(mousePosition);

            CheckHighlightedTile(gridCoordinate);

            if (!GameMaster.Instance.IsPlayerTurn)
                { return; }

            //try to select appropriate player character depending on mouse position
            TrySelectingMirroredPlayerCharacter(gridCoordinate);
            
            //DrawPathFromActiveEntityToMouse();

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TileBase clickedTile = _map.GetTile(gridCoordinate);
                if (clickedTile == null) {return;}

                OnTileSelected(gridCoordinate);
            }
            _previousTileMousedOver = gridCoordinate;
        }

        //this is simply to make the mouse refresh for a frame
        private void DirtyMousePosition()
        {
            Debug.Log("Dirtied mouse position");
            _previousTileMousedOver = new Vector3Int( 0, 0, -11);
        }

        private void TrySelectingMirroredPlayerCharacter(Vector3Int mouseCoordinate)
        {
            if (GameMaster.Instance.MultipleUniquePlayerCharacters) {return;}
            _mirrorManager.SelectActivePlayer(mouseCoordinate);
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
            RenderPlayerActionTile(null, tile);
            _mirrorManager.RenderMirroredSelection(_gm.ActiveEntity, tile);
            
        }

        public void RenderPlayerActionTile(EntityBase entity, TileNode tile)
        {
            if (entity == null)
                { entity = GameMaster.Instance.ActiveEntity; }
            if (!GameMaster.Instance.IsPlayerTurn || entity == null || entity?.CurrentTileNode == null || tile == null)
                { return; }
            //check range from active EntityCharacter
            int range = _mapManager.CalculateRange(tile.GridCoordinate, entity.CurrentTileNode.GridCoordinate);
            if ( range == 1)
            {
                if (tile.IsWalkable(entity))
                {
                     _mouseMap.SetTile(tile.GridCoordinate, _canMoveTileBase); 
                    return;
                }
                _shoveInteraction.TryRenderShoveHilight(entity.CurrentTileNode, tile);
            }
            if ( range == 2)
            {
                if (tile.IsWalkable(entity))
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

            if (tileNode.IsSelectable == false) 
                { return; }
            if (SelectedPlayerCharacter(tileNode))
            {
                RefreshOverlayMap();
                return;
            }
            EntityBase entity = GameMaster.Instance.ActiveEntity;
            if (TryToTakeAction(entity, tileNode))
            {   //if player successfully causes selected character to take action, force mirrored entity to also act.
                _mirrorManager.TryMirrorEntityAction(entity, tileNode);
            }
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
                // TileNode tile = GameMaster.Instance.ActiveEntity?.CurrentTileNode;
                // if (tile != null)
                // Debug.Log($"Tile is {_mapManager.CalculateRange(tile.GridCoordinate, gridCoordinate)} range from the Active Entity.");
            }
        }

        private bool SelectedPlayerCharacter(TileNode tileNode)
        {
            bool result = false;
            if (_gm.IsPlayerTurn == false || _gm.MultipleUniquePlayerCharacters == false) //used to include _gm.MultipleUniquePlayerCharacters == false, before mirror selection
                return false;

            
            EntityCharacter playerCharacter = tileNode?.GetPlayerCharacter();
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
            if (tile == null)
            {   //Invalid tile cast for mirror char, end their turn.
                HopEntity(entity, entity?.CurrentTileNode, 0);
                return true;
            }
            if (CanShoveTile(entity, tile))
            {                
                _shoveInteraction.ShoveThisTile(entity.CurrentTileNode, tile, 2);
                entity.ActionCompleted();
                return true;
            }
            if (CanMoveToTile(entity, tile, 1))
            {
                HopEntity(entity, tile, 1);
                return true;
            }
            if (CanMoveToTile(entity, tile, 2))
            {                //jump and shove
                HopEntity(entity, tile, 2);
                return true;                
            }

            if (_mapManager.CalculateRange(entity.CurrentTileNode.GridCoordinate, tile.GridCoordinate) <= 2)
            {
                //Requested action was within range but not on an appropriate tile (often requested as a mirrored action)
                //Entity ends turn not having done anything, but hops in place as feedback of spending their action.
                HopEntity(entity, entity?.CurrentTileNode, 0);
                return true;
            }

            //Entity could not do action, hop in place and end turn regardless.
            return false;
        }

        public bool CanShoveTile(EntityBase entity, TileNode tile)
        {
            Vector3Int entityPos = entity.CurrentTileNode.GridCoordinate;
            if ((_shoveInteraction.EntityOnThisTileThatCanBeShoved(tile)) && (_mapManager.CalculateRange(entityPos, tile.GridCoordinate) == 1))
            {
                return true;
            }
            return false;
        }
        public bool CanMoveToTile(EntityBase entity, TileNode tile, int range)
        {
            if (tile == null || tile?.IsWalkable(entity) == false)
                { return false; }
            Vector3Int entityPos = entity.CurrentTileNode.GridCoordinate;
            if (_mapManager.CalculateRange(entityPos, tile.GridCoordinate) > range)
                { return false; }
            return true;
        }

        public void HopEntity(EntityBase entity, TileNode targetTile, int range)
        {
            bool slamLanding = false;
            if (entity?.CurrentTileNode == null || targetTile == null)
                { return; }
            Vector3 position = targetTile.WorldPos;

            if (range > 1)
                { slamLanding = true; }

            // entity.LeaveTileNode();
            StartCoroutine(DoHopEntityToPos(entity, targetTile, _slideSpeed, slamLanding));
        }

        IEnumerator DoHopEntityToPos(EntityBase entity, TileNode targetTile, float duration, bool slamAtEnd)
        {
            SoundManager.Instance.PlaySound(entity.JumpTakeoff);
            Vector3 targetPosition = targetTile.WorldPos;
            if (entity == null) { yield break; }
            GameMaster.Instance.TilemapInteractable = false;
            GameMaster.Instance.AddEntityInMotion(entity);

            float timeElapsed = 0;
            Vector3 startPos = entity.transform.position;
            while (timeElapsed < duration)
            {
                if (entity == null)
                {
                    GameMaster.Instance.TilemapInteractable = true;
                    GameMaster.Instance.RemoveEntityInMotion(entity);
                    yield break;
                }

                float t = timeElapsed / duration;
                t = t * t * (3f - 2f *t);
                float g = timeElapsed/duration;
                g = 1 - ((1 - g)*(1 - g));


                float x = Mathf.Lerp(startPos.x, targetPosition.x, g);
                float hopHeight = ((startPos.y + targetPosition.y) * 0.5f) +1;
                float launch = Mathf.Lerp(startPos.y, hopHeight, g);
                float land = Mathf.Lerp(hopHeight, targetPosition.y, g);
                float y = Mathf.Lerp(launch, land, t);

                entity.transform.position = new Vector3(x, y, 0);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
            
            entity.transform.position = targetPosition; 
            entity.LinkToTileNode(null);
            entity.SnapEntityPositionToTile();

            if (slamAtEnd)
            {
                entity.GetComponent<JumpAndShove>()?.ActivateJumpPushback();
            }

            SoundManager.Instance.PlaySound(entity.HopLanding);
            entity.ActionCompleted();
            GameMaster.Instance.TilemapInteractable = true;
            GameMaster.Instance.RemoveEntityInMotion(entity);
        }
        
        public void RenderTriggerHilight(Vector3Int tileCoords, TileBase triggerTileHilight)
        {
            if (_triggerTileMap == null) { return; }
            _triggerTileMap.SetTile(tileCoords, triggerTileHilight);
        }
        public void ClearTriggerHilight(Vector3Int tileCoords)
        {
            if (_triggerTileMap == null) { return; }
            _triggerTileMap.SetTile(tileCoords, null);
        }

        private void OnDisable()
        {
            _turnManager.OnPlayerTurnBegins -= DirtyMousePosition;
        }
    }
    
}
