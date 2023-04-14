using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
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
        [SerializeField] private Tilemap _triggerTilemap;
        public Tilemap TriggerTilemap => _triggerTilemap;
        [SerializeField] private Tilemap _mouseInteractionTilemap;
        public Tilemap MouseInteractionTilemap => _mouseInteractionTilemap;
        [SerializeField] private Tilemap _occlusionTilemap;
        public Tilemap OcclusionTilemap => _occlusionTilemap;
        private MapInteractionManager _mapInteractionManager;
        public MapInteractionManager MapInteractionManager => _mapInteractionManager;
        private MoveEntityAlongPath _moveEntityAlongAPath;
        private TileNodeManager _tileNodeManager;
        public TileNodeManager TileNodeManager => _tileNodeManager;

        private void Awake()
        {
            ActivateTilemaps();
            _mapInteractionManager = GetComponent<MapInteractionManager>();
            _moveEntityAlongAPath = GetComponent<MoveEntityAlongPath>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _mapInteractionManager.Initialize(this);
            InitializeTileNodeManager(_map);
        }

        private void ActivateTilemaps()
        {
            _overlayTilemap?.gameObject.SetActive(true);
            _triggerTilemap?.gameObject.SetActive(true);
            _mouseInteractionTilemap?.gameObject.SetActive(true);
            _occlusionTilemap?.gameObject.SetActive(true);
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
            //This is using odd-row offset coordinates
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

        public Vector3Int CastOddRowToAxial(Vector3Int oddRow)
        {
            //Can convert from columns and rows into axial coordinates
            //axial cooridates are 3 axis: q(column), r(row), s(3rd axis)
            //when all three of the axis are added together they will equal 0
            //this means we can ignore s because it is always equal to -q-r
            int q = oddRow.x - (oddRow.y - (oddRow.y&1)) /2;
            int r = oddRow.y;
            int s = -q-r;
            return new Vector3Int(q, r, s);
        } 

        public Vector3Int CastAxialToOddRow(Vector3Int axial)
        {
            int column = axial.x + (axial.y - (axial.y&1)) /2;
            int oddRow = axial.y;
            return new Vector3Int (column, oddRow, 0);
        }

        public int CalculateRange(Vector3Int coordA, Vector3Int coordB)
        {
            Vector3Int a = CastOddRowToAxial(coordA);
            Vector3Int b = CastOddRowToAxial(coordB);

            Vector3Int vec = a-b;
            int range = (Mathf.Abs(vec.x) + Mathf.Abs(vec.y) + Mathf.Abs(vec.z)) / 2;
            return range;
        }

        public TileNode GetTileNodeAtWorldPos(Vector3 position)
        {
            position.z = 0;
            Vector3Int gridCoordinate = _map.WorldToCell(position);
            return _tileNodeManager.GetNodeFromCoords(gridCoordinate);
        }

        public Vector3 GetWorldPosFromGridCoord(Vector3Int gridCoord)
        {
            return _map.CellToWorld(gridCoord);
        }

        public Vector3Int CalculateAxialPointerBetweenTiles(TileNode originTile, TileNode targetTile)
        {
            Vector3Int originPos = CastOddRowToAxial(originTile.GridCoordinate);
            Vector3Int targetPos = CastOddRowToAxial(targetTile.GridCoordinate);

            Vector3Int pointingVector = targetPos - originPos;

            return pointingVector;
        }
    }
}
