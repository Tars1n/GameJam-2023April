using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using GameJam.Pathfinding;

namespace GameJam.Map
{
    [RequireComponent(typeof(MapInteractionManager), typeof(TileNodeManager))]
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = true;
        [SerializeField] private Tilemap _map;
        public Tilemap Map => _map;
        [SerializeField] private Tilemap _overlayTilemap;
        public Tilemap OverlayMap => _overlayTilemap;
        [SerializeField] private Tilemap _mouseInteractionTilemap;
        public Tilemap MouseInteractionTilemap => _mouseInteractionTilemap;
        private MapInteractionManager _mapInteractionManager;
        public MapInteractionManager MapInteractionManager => _mapInteractionManager;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private TileNodeManager _tileNodeManager;
        public TileNodeManager TileNodeManager => _tileNodeManager;

        private void Awake()
        {
            _mapInteractionManager = GetComponent<MapInteractionManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _mapInteractionManager.Initialize(this);
            InitializeTileNodeManager(_map);
        }

        private void InitializeTileNodeManager(Tilemap mapToDesignate)
        {
            _map.CompressBounds();
            if (_debugLogs)
                Debug.Log($"Tilemap generated map of {_map.size.x}x by {_map.size.y}y");
            _tileNodeManager.InitializeTileNodeArray(_map);
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
            Vector3Int gridCoordinate = _map.WorldToCell(position);
            return _tileNodeManager.GetNodeFromCoords(gridCoordinate);
        }
    }
}
