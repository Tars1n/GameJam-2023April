using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using GameJam.Pathfinding;

namespace GameJam.Map
{
    [RequireComponent(typeof(PathfindingManager), typeof(TileNodeManager))]
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = true;
        [SerializeField] private Tilemap _map;
        [SerializeField] private int _mp = 3;
        public Tilemap Map => _map;
        [SerializeField] private Tilemap _overlayTilemap;
        [SerializeField] private TileBase _canMoveTileBase;
        [SerializeField] private TileBase _selectionTileBase;
        [SerializeField] private TileBase _activeEntityTileBase;        
        private PathfindingManager _pathfinding;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private TileNodeManager _tileNodeManager;
        public TileNodeManager TileNodeManager => _tileNodeManager;

        private void Awake()
        {
            _pathfinding = GetComponent<PathfindingManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            InitializeTileNodeManager(_map);
        }

        private void InitializeTileNodeManager(Tilemap mapToDesignate)
        {
            _map.CompressBounds();
            if (_debugLogs)
                Debug.Log($"Tilemap generated map of {_map.size.x}x by {_map.size.y}y");
            _tileNodeManager.InitializeTileNodeArray(_map);
        }

        private void Update() {
            if (GameMaster.Instance.TilemapInteractable == false)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3Int gridPosition = _map.WorldToCell(mousePosition);

                TileBase clickedTile = _map.GetTile(gridPosition);
                if (clickedTile == null) {return;}

                if (_debugLogs)
                    Debug.Log($"Clicked on tile {clickedTile} at coordinates: {gridPosition.x}, {gridPosition.y}.");

                OnTileSelected(gridPosition);
            }
        }

        public void OnTileSelected(Vector3Int gridPosition)
        {
            if (_debugLogs)
            {
                Vector3Int arrayPos = _tileNodeManager.ConvertCoordsToArrayIndex(gridPosition);
                TileNode tileNode = _tileNodeManager.GetNodeAtArrayIndex(arrayPos);
                Debug.Log($"clicked gird pos {gridPosition}, array pos {arrayPos}, node {tileNode}");            
                if (tileNode == null)
                {
                    Debug.Log($"node is null");
                }
            }
            TileBase selectedTile = _overlayTilemap.GetTile(gridPosition);
            if (selectedTile == _canMoveTileBase)
            {
                _moveEntityAlongAPath.MoveEntityAlongPathFunc(gridPosition);
                RefreshOverlayMap();
                return;
            }
            if (selectedTile)
            {
                //tile already selected, deselecting
                RefreshOverlayMap();
                return;
            }

            _overlayTilemap.ClearAllTiles();
            _overlayTilemap.SetTile(gridPosition, _selectionTileBase);
            _pathfinding?.FillPathMPNotBlockedByObstacles(gridPosition, _mp);
            HighlightActiveEntityTile();
        }

        public void RefreshOverlayMap()
        {
            _overlayTilemap.ClearAllTiles();
            HighlightActiveEntityTile();
        }

        private void HighlightActiveEntityTile()
        {
            Entity.EntityBase entity = GameMaster.Instance.ActiveEntity;
            TileNode tile = entity?.CurrentTileNode;
            if (tile != null)
            {
                _overlayTilemap.SetTile(tile.GridPosition, _activeEntityTileBase);
            }
        }

        public Vector3Int[] GetAllAdjacentHexCoordinates(Vector3Int startingPosition)
        {   //Array of directions pointing from top left going counter-clockwise
            Vector3Int[] directions = new Vector3Int[6];

            //TODO move this calculation to a HelperClass
            if (startingPosition.y % 2 == 0)
            {
                directions[0] = new Vector3Int(-1,  1,  0); //Upleft
                directions[2] = new Vector3Int(-1,  0,  0); //Left
                directions[1] = new Vector3Int(-1, -1,  0); //Downleft
                directions[3] = new Vector3Int( 0, -1,  0); //Downright
                directions[5] = new Vector3Int( 1,  0,  0); //Right
                directions[4] = new Vector3Int( 0,  1,  0); //Upright
            } else {
                directions[0] = new Vector3Int(0, 1, 0); //Upleft
                directions[2] = new Vector3Int(-1, 0, 0); //Left
                directions[1] = new Vector3Int(0, -1, 0); //Downleft
                directions[3] = new Vector3Int(1, -1, 0); //Downright
                directions[4] = new Vector3Int(1, 1, 0); //upright
                directions[5] = new Vector3Int(1, 0, 0); //Right
            }

            return directions;
        }

        public Vector3Int GetAdjacentHexCoordinate(Vector3Int startingPosition, int direction)
        {
            if (direction > 5 || direction < 0)
            {
                Debug.LogError("Can only ask for directions between 0-5. Direction starts from the top left at 0 and goes around the hex counter-clockwise.");
                return new Vector3Int(0,0,0);
            }
            return GetAllAdjacentHexCoordinates(startingPosition)[direction];
        }

        public TileNode GetTileNodeAtWorldPos(Vector3 position)
        {
            position.z = 0;
            Vector3Int gridPosition = _map.WorldToCell(position);
            return _tileNodeManager.GetNodeFromCoords(gridPosition);
        }
    }
}
