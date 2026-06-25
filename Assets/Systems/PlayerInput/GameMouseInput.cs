using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.PlayerInput
{
    /**
    @Author: Luke Johson
    Uses the Input system to get the mouse pos and if the left mouse button is clicked.
    Does not use an update  method, it is called by other scripts when the mouse info is needed.
    functions return either null or the tile coordinates.
    **/
    public class GameMouseInput : GameInput
    {

        public InputActionAsset InputAsset;
        private InputAction mMousePosAction;
        private InputAction mMouseClickAction;
        private MapManager _mapManager;
        private Tilemap _map;


        void OnEnable()
        {
            InputAsset.FindActionMap("Player").Enable();
        }
        void OnDisable()
        {
            InputAsset.FindActionMap("Player").Disable();
        }
        void Awake()
        {
            mMousePosAction = InputSystem.actions.FindAction("Mouse_Pos");
            mMouseClickAction = InputSystem.actions.FindAction("Mouse_Click");
            _mapManager = GetComponent<MapManager>();
            _map = _mapManager.Map;
        }
        /**
        return the tile the mouse is over.
        **/
        public override Vector3Int? GetInputPosForHilight()
        {
            //get pos of mouse
            UnityEngine.Vector2 mousePosition = mMousePosAction.ReadValue<UnityEngine.Vector2>();
            //convert to game vector
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            //convert to tile pos
            Vector3Int gridCoordinate = _map.WorldToCell(mousePosition);
            return gridCoordinate;
        }
        /**
        if mouse pressed this frame, return the tile the mouse is over.
        **/
        public override Vector3Int? GetInputBoolForMove()
        {
            //mouse was pressed so return tile
            if (mMouseClickAction.WasPressedThisFrame())
            {
                //get pos of mouse
                UnityEngine.Vector2 mousePosition = mMousePosAction.ReadValue<UnityEngine.Vector2>();
                //convert to game vector
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
                //convert to tile pos
                Vector3Int gridCoordinate = _map.WorldToCell(mousePosition);
                return gridCoordinate;
            }
            //mouse was not pressed so return null
            return null;
        }
    }
}
