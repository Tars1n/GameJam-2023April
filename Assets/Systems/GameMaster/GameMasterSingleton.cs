using UnityEngine;
using System;
using GameJam.Map;

namespace GameJam
{
    [RequireComponent(typeof(ReferenceManager),typeof(EventManager))]
    public class GameMasterSingleton : MonoBehaviour
    {
        private ReferenceManager _referenceManager;
        public ReferenceManager ReferenceManager => _referenceManager;
        private EventManager _eventManager;
        public EventManager EventManager => _eventManager;
        public bool _jacobLogs = false;
        public bool _lukeLogs = false;
        [SerializeField] bool _gmLogs = false;
        [SerializeField] private float _currentTimeScale;
        private float _fixedDeltaTime;
        [SerializeField] private GameObject _activeUnit;
        public GameObject ActiveUnit => _activeUnit;
        [SerializeField] private TileNode _selectedTile = null;
        public TileNode SelectedTile => _selectedTile;
        

        private void Awake() {
            _fixedDeltaTime = Time.fixedDeltaTime;
            _referenceManager = GetComponent<ReferenceManager>();
            _eventManager = GetComponent<EventManager>();            
            DontDestroyOnLoad(this);
        }

        public void SetSelectedTile(TileNode node)
        {
            _selectedTile?.DeselectNode();

            if (_selectedTile == node) 
            {   //tile already selected, deselecting
                _selectedTile = null;
                return;
            }

            _selectedTile = node;
            _selectedTile?.SelectNode();
        }

        public void SetTimescale(float scale)
        {
            _currentTimeScale = scale;
            Time.timeScale = _currentTimeScale;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        public void PauseTimescale()
        {
            if (_gmLogs)
                Debug.Log("pausing timescale.");
            SetTimescale(0.0f);
        }
        
        public void NormalizeTimescale()
        {
            if (_gmLogs)
                Debug.Log("returning timescale to normal.");
            SetTimescale(1f);
        }
    }
}
