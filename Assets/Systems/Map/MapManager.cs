using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace GameJam.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _map;
        [SerializeField] private Tilemap _overlayTilemap;
        [SerializeField] private TileBase _selectionTileBase;

        private void Update() {
            if (GameMaster.Instance.TilemapInteractable == false)
                return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                Vector3Int gridPosition = _map.WorldToCell(mousePosition);

                TileBase clickedTile = _map.GetTile(gridPosition);
                if (clickedTile == null) {return;}

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
        }

        private TileBase[] GetAdjacentHexTiles(Vector3Int gridPosition)
        {
            TileBase[] tiles = new TileBase[6];
            Vector3Int[] adjacentTiles = GetAdjacentHexCoordinates(gridPosition);

            for (int i = 0; i < 6; i++)
            {
                tiles[i] = _map.GetTile(gridPosition + adjacentTiles[i]);
            }

            return tiles;
        }

        public Vector3Int[] GetAdjacentHexCoordinates(Vector3Int startingPosition)
        {   //Array of directions pointing from top left going counter-clockwise
            Vector3Int[] directions = new Vector3Int[6];

            if (startingPosition.y % 2 == 0)
            {
                directions[0] = new Vector3Int( 0,  1,  0); //Upleft
                directions[2] = new Vector3Int(-1,  0,  0); //Left
                directions[1] = new Vector3Int( 0, -1,  0); //Downleft
                directions[3] = new Vector3Int( 1, -1,  0); //Downright
                directions[4] = new Vector3Int( 1,  1,  0); //upright
                directions[5] = new Vector3Int( 1,  0,  0); //Right
            } else {
                directions[0] = new Vector3Int(-1,  1,  0); //Upleft
                directions[2] = new Vector3Int(-1,  0,  0); //Left
                directions[1] = new Vector3Int(-1, -1,  0); //Downleft
                directions[3] = new Vector3Int( 0, -1,  0); //Downright
                directions[5] = new Vector3Int( 1,  0,  0); //Right
                directions[4] = new Vector3Int( 0,  1,  0); //Upright
            }

            return directions;
        }
    }
}
