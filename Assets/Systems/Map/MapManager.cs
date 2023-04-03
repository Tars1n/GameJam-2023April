using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using GameJam.Pathfinding;

namespace GameJam.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = true;
        [SerializeField] private Tilemap _map;
        public Tilemap Map => _map;
        [SerializeField] private Tilemap _overlayTilemap;
        [SerializeField] private TileBase _selectionTileBase;        
        private PathfindingManager _pathfinding;

        private void Awake()
        {
            _pathfinding = GetComponent<PathfindingManager>();
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

        private void OnTileSelected(Vector3Int gridPosition)
        {
            TileBase selectedTile = _overlayTilemap.GetTile(gridPosition);
            if (selectedTile)
            {
                //tile already selected, deselecting
                _overlayTilemap.ClearAllTiles();
                return;
            }

            _overlayTilemap.ClearAllTiles();
            _overlayTilemap.SetTile(gridPosition, _selectionTileBase);
            _pathfinding?.FillPathMP(gridPosition);
        }

        private TileNode[] GetAdjacentHexTiles(Vector3Int gridPosition)
        {   //TODO: Change this function to fetch TileNode info from the 2DArray?
            TileNode[] tiles = new TileNode[6]; //!should not constantly create new TileNodes. Probably better to create them on the 2DArray and reference those.
            Vector3Int[] adjacentTiles = GetAllAdjacentHexCoordinates(gridPosition);

            for (int i = 0; i < 6; i++)
            {
                Vector3Int coord = gridPosition + adjacentTiles[i];
                tiles[i].GridPosition = coord;
                tiles[i].TileType = _map.GetTile(coord);
            }

            return tiles;
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
    }
}