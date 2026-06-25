using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam.PlayerInput
{
    /**
    @Author: Luke Johson
    Uses the Input system to get the mouse pos and if the left mouse button is clicked.
    Does not use an update  method, it is called by other scripts when the mouse info is needed.
    **/
    public class GameMouseInput : MonoBehaviour, IGameInput
    {

        public InputActionAsset InputAsset;
        private InputAction mMousePosAction;
        private InputAction mMouseClickAction;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

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
        }
        public UnityEngine.Vector2 GetInputPosForHilight()
        {
            return mMousePosAction.ReadValue<UnityEngine.Vector2>();
        }
        public bool GetInputBoolForMove()
        {
            return mMouseClickAction.WasPressedThisFrame();
        }
    }
}
