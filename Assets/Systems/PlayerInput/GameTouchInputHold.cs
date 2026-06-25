using UnityEngine;
using UnityEngine.InputSystem;
using GameJam.Map;
using UnityEngine.Tilemaps;

namespace GameJam.PlayerInput
{
    /**
    @Author: Luke Johson
    Uses the Input system to get the touch pos and if touch is held, this returns the GetInputPosForHilight.
    Also uses the input system to get the touch pos when released, this returns the GetInputBoolForMove.
    Does not use an update  method, it is called by other scripts when the mouse info is needed.
    functions return either null or the tile coordinates.
    **/
    public class GameTouchInputHold : GameInput
    {

        public InputActionAsset InputAsset;
        private InputAction mTouchPosAction;
        private InputAction mTouchPressAction;
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
            mTouchPosAction = InputSystem.actions.FindAction("Touch_Pos");
            mTouchPressAction = InputSystem.actions.FindAction("Touch_Press");
            _mapManager = GetComponent<MapManager>();
            _map = _mapManager.Map;
        }

        /**
        return the tile touch is held on.
        **/
        public override Vector3Int? GetInputPosForHilight()
        {
            UnityEngine.Debug.Log($"in touch hold");
            if (mTouchPressAction.IsPressed())
            {
                UnityEngine.Debug.Log($"touch started");
            }
            return null;
        }

        public override Vector3Int? GetInputBoolForMove()
        {
            return null;
        }
    }
}