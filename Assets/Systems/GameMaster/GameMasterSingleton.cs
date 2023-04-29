using UnityEngine;
using System.Collections.Generic;
using System;
using GameJam.Map;
using GameJam.Entity;
using GameJam.Level.Scene;

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
        public bool InCutscene = false;
        [SerializeField] private float _currentTimeScale;
        private float _fixedDeltaTime;
        [SerializeField] private EntityBase _activeEntity;
        public EntityBase ActiveEntity => _activeEntity;
        public bool MultipleUniquePlayerCharacters = false; //controls and turn system changes slightly if you need to select between completely unique squad members
        public bool IsPlayerTurn => _referenceManager.TurnManager.PlayerTurn;
        public bool TilemapInteractable = false;
        [SerializeField] private List<EntityBase> _entitiesInMotion = new List<EntityBase>();
        public List<EntityBase> EntitiesInMotion => _entitiesInMotion;
        private GameObject _pauseIcon; //temp bug fix thingie
        private GameObject PauseIcon => _pauseIcon ? _pauseIcon : _pauseIcon = GameObject.Find("PauseIcon");
            

        private void Awake() {
            _fixedDeltaTime = Time.fixedDeltaTime;
            _referenceManager = GetReferenceManager();
            _eventManager = GetComponent<EventManager>();
            DontDestroyOnLoad(this);
            Initialize();
        }

        public void Initialize()
        {
            TilemapInteractable = false;
            _entitiesInMotion = new List<EntityBase>();
            _activeEntity = null;
            _pauseIcon = null;
            Debug.Log("GameMaster initialized.");
        }

        public void EndScene()
        {
            Initialize();
            ReferenceManager.LevelManager.transform.root.BroadcastMessage("StopAllCoroutines");
            ReferenceManager.StopAllCoroutinesEverywhere();
            ReferenceManager.TileNodeManager.DeleteAllTileNodes();
            // Destroy(_referenceManager);
            // _referenceManager = null;
            // GameSuspended = true;
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
            ReferenceManager.MapInteractionManager?.RefreshOverlayMap();
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

        public void AddEntityInMotion(EntityBase entity)
        {
            _entitiesInMotion.Add(entity);
            PauseIcon?.SetActive(true);
        }

        public void RemoveEntityInMotion(EntityBase entity)
        {
            if (entity == null)
                { _entitiesInMotion.Clear();}
            _entitiesInMotion.Remove(entity);
            _entitiesInMotion.TrimExcess();
            if (_entitiesInMotion.Count == 0 && _referenceManager.TurnManager.HoldingNextTick)
            {
                PauseIcon?.SetActive(false);
                _referenceManager.TurnManager.TickNext();
            }
        }
    }
}
