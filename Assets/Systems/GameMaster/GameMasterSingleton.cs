using UnityEngine;
using System;
using GameJam.Map;

namespace GameJam
{
    [RequireComponent(typeof(EventManager))]
    public class GameMasterSingleton : MonoBehaviour
    {
        private ReferenceManager _referenceManager;
        public ReferenceManager ReferenceManager => GetReferenceManager();
        private EventManager _eventManager;
        public EventManager EventManager => _eventManager;
        public bool _jacobLogs = false;
        public bool _lukeLogs = false;
        [SerializeField] bool _gmLogs = false;
        public bool GameSuspended = false;
        [SerializeField] private float _currentTimeScale;
        private float _fixedDeltaTime;
        [SerializeField] private Entity.EntityBase _activeUnit;
        public Entity.EntityBase ActiveUnit => _activeUnit;
        [SerializeField] private TileGameObject _selectedTile = null;
        public TileGameObject SelectedTile => _selectedTile;
        public bool TilemapInteractable = true;
        

        private void Awake() {
            _fixedDeltaTime = Time.fixedDeltaTime;
            _referenceManager = GetReferenceManager();
            _eventManager = GetComponent<EventManager>();          
            DontDestroyOnLoad(this);
        }

        public ReferenceManager GetReferenceManager()
        {
            if (_referenceManager == null)
            {
                GameObject newRef = new GameObject("ReferenceManager");
                newRef.AddComponent<ReferenceManager>();
                _referenceManager = newRef.GetComponent<ReferenceManager>();
            }

            return _referenceManager;
        }

        public void SetSelectedTile(TileGameObject node)
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
            GameSuspended = true;
            SetTimescale(0.0f);
        }
        
        public void NormalizeTimescale()
        {
            if (_gmLogs)
                Debug.Log("returning timescale to normal.");
            SetTimescale(1f);
            GameSuspended = false;
        }
    }
}
