using UnityEngine;
using System;
using GameJam.Map;
using GameJam.Entity;

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
        [SerializeField] private EntityBase _activeEntity;
        public EntityBase ActiveEntity => _activeEntity;
        public bool MultiplePlayerCharacters = true; //controls and turn system changes slightly if you need to select between characters
        public bool IsPlayerTurn => _referenceManager.TurnManager.PlayerTurn;
        public bool TilemapInteractable = false;
        

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

        public void SetActiveEntity(EntityBase entity)
        {
            _activeEntity = entity;
            ReferenceManager.MapInteractionManager.RefreshOverlayMap();
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
