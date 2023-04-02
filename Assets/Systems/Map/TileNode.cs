using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace GameJam.Grid
{
    public class TileNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private Tilemap _tilemap;
        public Vector3Int TilePosition {get; private set;}
        [SerializeField] private bool _evenTileNode;
        [SerializeField] Vector3Int[] _direction;
        [SerializeField] TileNode[] _neighbours;

        //hold unit data for tile
        private SpriteRenderer _spriteRenderer;
        private Color _startColour;
        [SerializeField] private Color _hoverColour;
        [SerializeField] private Color _selectedColour;
        private bool _nodeSelected = false;

        private void Awake() {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _startColour = _spriteRenderer.color;
            _tilemap = transform.parent.GetComponent<Tilemap>();
            if (_tilemap == null)
            {
                Debug.LogError("Could not locate TileMap");
                return;
            }
            TilePosition = _tilemap.WorldToCell(this.transform.position);
        }

        private void Start()
        {
            GetAdjacentTiles();
        }

        private void GetAdjacentTiles()
        {
            _direction = new Vector3Int[6];
            if (TilePosition.y % 2 > 0)
            {
                _evenTileNode = false;
                _direction[0] = new Vector3Int( 1, -1,  0); //upleft
                _direction[1] = new Vector3Int( 0, -1,  0); //downleft
                _direction[2] = new Vector3Int(-1,  0,  0); //down
                _direction[3] = new Vector3Int( 0,  1,  0); //downright
                _direction[4] = new Vector3Int( 1,  1,  0); //upright
                _direction[5] = new Vector3Int( 1,  0,  0); //up
            } else {
                _evenTileNode = true;
                _direction[0] = new Vector3Int( 0, -1,  0); //upleft
                _direction[1] = new Vector3Int(-1, -1,  0); //downleft
                _direction[2] = new Vector3Int(-1,  0,  0); //down
                _direction[3] = new Vector3Int(-1,  1,  0); //downright
                _direction[4] = new Vector3Int( 0,  1,  0); //upright
                _direction[5] = new Vector3Int( 1,  0,  0); //up
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHoverState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ExitHoverState();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            OnClickedTile();
        }

        private void OnHoverState()
        {
            if (_nodeSelected) {return;}
            
            _spriteRenderer.color = _hoverColour;
        }

        private void ExitHoverState()
        {
            if (_nodeSelected) {return;}

            _spriteRenderer.color = _startColour;
        }

        private void OnClickedTile()
        {
            Debug.Log("Pointer Clicked on tile: " + TilePosition );

            if (_nodeSelected)
            {
                _spriteRenderer.color = _hoverColour;
                _nodeSelected = false;
                return;
            }

            if (!_nodeSelected)
            {   
                _spriteRenderer.color = _selectedColour;
                _nodeSelected = true;
                return;
            }
        }
    }
}
