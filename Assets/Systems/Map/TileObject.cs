using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GameJam.TileMap
{
    public class TileObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer entered object.");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer exited object.");
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer Clicked Object");
        }
    }
}
