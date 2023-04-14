using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace GameJam.PlayerInput
{
    public class ClickManager : MonoBehaviour
    {
        private bool _debugLogs = false;
        private MapManager _mapManager;
        [SerializeField] private Tilemap _map;
        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _map = _mapManager.Map;
            if (_debugLogs){}
        }
        // private void Update()
        // {
        //     if (GameMaster.Instance.TilemapInteractable == false)
        //         return;

        //     if (Mouse.current.leftButton.wasPressedThisFrame)
        //     {
        //         Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        //         Vector3Int gridPosition = _map.WorldToCell(mousePosition);

        //         TileBase clickedTile = _map.GetTile(gridPosition);
        //         if (clickedTile == null) { return; }

        //         if (_debugLogs)
        //             Debug.Log($"Clicked on tile {clickedTile} at coordinates: {gridPosition.x}, {gridPosition.y}.");

        //         _mapManager.OnTileSelected(gridPosition);
        //     }
        // }
    }
}
