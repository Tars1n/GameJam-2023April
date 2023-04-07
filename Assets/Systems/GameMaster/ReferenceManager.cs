using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;
using GameJam.Pathfinding;

namespace GameJam
{
    public class ReferenceManager : MonoBehaviour
    { //so far might be able to just use LevelManager instead of reference manager?
        [SerializeField] private LevelManager _levelManager;
        public LevelManager LevelManager => _levelManager;
        [SerializeField] private MapManager _mapManager;
        public MapManager MapManager => _mapManager;
        [SerializeField] private MapInteractionManager _mapInteractionManager;
        public MapInteractionManager MapInteractionManager => _mapInteractionManager;
        [SerializeField] private TurnManager _turnManager;
        public TurnManager TurnManager => _turnManager;
        [SerializeField] private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;
        [SerializeField] private MoveEntityAlongPath _plotPath;
        public MoveEntityAlongPath PlotPath => _plotPath;
        
        
        private void Awake()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            if (_levelManager == null)
            {   
                Debug.LogError("ReferenceManager failed to Find LevelManager.");
                return;
            }
            GrabLevelReferences();
        }
        public void GrabLevelReferences()
        {
            _mapManager = _levelManager?.MapManager;
            _mapInteractionManager = _mapManager?.MapInteractionManager;
            _turnManager = _levelManager?.GetComponent<TurnManager>();
            _entityManager = _turnManager?.GetComponent<EntityManager>();
        }
        
    }
}
