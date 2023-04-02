using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    public class TileNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        private Tilemap _tilemap;
        public Vector3Int TilePosition {get; private set;}
        [SerializeField] private bool _evenTileNode;
        [SerializeField] Vector3Int[] _direction = new Vector3Int[6];

        //hold unit data for tile
        private SpriteRenderer _spriteRenderer;
        private Color _startColour;
        [SerializeField] private Color _evenHexColour;
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
            DefineHexDirections();
        }

        private void DefineHexDirections()
        {

            if (TilePosition.y % 2 == 0)
            {
                _evenTileNode = false;
                _direction[0] = new Vector3Int( 0,  1,  0); //Upleft
                _direction[2] = new Vector3Int(-1,  0,  0); //Left
                _direction[1] = new Vector3Int( 0, -1,  0); //Downleft
                _direction[3] = new Vector3Int( 1, -1,  0); //Downright
                _direction[4] = new Vector3Int( 1,  1,  0); //upright
                _direction[5] = new Vector3Int( 1,  0,  0); //Right
            } else {
                _evenTileNode = true;
                _direction[0] = new Vector3Int(-1,  1,  0); //Upleft
                _direction[2] = new Vector3Int(-1,  0,  0); //Left
                _direction[1] = new Vector3Int(-1, -1,  0); //Downleft
                _direction[3] = new Vector3Int( 0, -1,  0); //Downright
                _direction[5] = new Vector3Int( 1,  0,  0); //Right
                _direction[4] = new Vector3Int( 0,  1,  0); //Upright
            }
            
            if (TilePosition.x % 2 != 0 && !_evenTileNode)
            {   //offset tile colour for a nicer look
                _spriteRenderer.color = _startColour = _evenHexColour;
            }
        }

        public Vector3Int GetHexNeighbourCoordinatesAt(int direction)
        {  
            Vector3Int neighbourCoordinates = new Vector3Int();
            neighbourCoordinates = TilePosition + _direction[direction];

            return neighbourCoordinates;
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
            if (GameMaster.Instance._jacobLogs)
                Debug.Log("Pointer Clicked on tile: " + TilePosition );
            GameMaster.Instance.SetSelectedTile(this);
        }

        public void SelectNode()
        {
            if (!_nodeSelected)
            {   
                _spriteRenderer.color = _selectedColour;
                _nodeSelected = true;
                return;
            }
        }

        public void DeselectNode()
        {
            if (_nodeSelected)
            {
                _spriteRenderer.color = _startColour;
                _nodeSelected = false;
                return;
            }
        }
    }
}
