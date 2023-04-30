using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;
using GameJam.Pathfinding;
using GameJam.Level;
using GameJam.Dialogue;
using GameJam.UI;

namespace GameJam
{
    public class ReferenceManager : MonoBehaviour
    { //so far might be able to just use LevelManager instead of reference manager?
        [SerializeField] private LevelManager _levelManager;
        public LevelManager LevelManager => _levelManager;
        [SerializeField] private MapManager _mapManager;
        public MapManager MapManager => _mapManager;
        [SerializeField] private TileNodeManager _tileNodeManager;
        public TileNodeManager TileNodeManager => _tileNodeManager;
        [SerializeField] private MapInteractionManager _mapInteractionManager;
        public MapInteractionManager MapInteractionManager => _mapInteractionManager;
        [SerializeField] private PathfindingManager _pathfindingManager;
        public PathfindingManager PathFindingManager => _pathfindingManager;        
        [SerializeField] private TurnManager _turnManager;
        public TurnManager TurnManager => _turnManager;
        [SerializeField] private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;
        [SerializeField] private DialogueManager _dialogueManager;
        public DialogueManager DialogueManager => _dialogueManager;
        [SerializeField] private ScoreDisplay _scoreDisplay;
        public ScoreDisplay ScoreDisplay => _scoreDisplay;
        // [SerializeField] private MoveEntityAlongPath _plotPath;
        // public MoveEntityAlongPath PlotPath => _plotPath;
        
        
        private void Awake()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            if (_levelManager == null)
            {   
                Debug.LogError("ReferenceManager failed to Find LevelManager.");
                return;
            }
            GrabLevelReferences();
            ValidateReferences();
        }
        public void GrabLevelReferences()
        {
            _mapManager = _levelManager?.MapManager;
            _mapInteractionManager = _mapManager?.MapInteractionManager;
            _tileNodeManager = _mapManager?.GetComponent<TileNodeManager>();
            _pathfindingManager = _mapManager?.GetComponent<PathfindingManager>();
            _turnManager = _levelManager?.GetComponent<TurnManager>();
            _entityManager = _turnManager?.GetComponent<EntityManager>();
            _dialogueManager = _levelManager?.GetComponent<DialogueManager>(); //this value is stored in the inspector in levelmanager
            _scoreDisplay = _levelManager?.GetComponent<ScoreDisplay>();
        }

        private void ValidateReferences()
        {
            if (_levelManager == null)
                Debug.LogError("ReferenceManager could not locate LevelManager!");
            if (_mapManager == null)
                Debug.LogError("ReferenceManager could not locate MapManager!");
            if (_mapInteractionManager == null)
                Debug.LogError("ReferenceManager could not locate MapInteractionManager!");
            if (_tileNodeManager == null)
                Debug.LogError("ReferenceManager could not locate TileNodeManager!");
            if (_pathfindingManager == null)
                Debug.LogError("ReferenceManager could not locate PathfindingManager!");
            if (_turnManager == null)
                Debug.LogError("ReferenceManager could not locate TurnManager!");
            if (_entityManager == null)
                Debug.LogError("ReferenceManager could not locate EntityManager!");
            if (_dialogueManager == null)
                Debug.LogError("ReferenceManager could not locate DialogueManager");
        }

        public void StopAllCoroutinesEverywhere()
        {
            LevelManager?.StopAllCoroutines();
            _mapInteractionManager.StopAllCoroutines();
            _mapInteractionManager.MapShoveInteraction.StopAllCoroutines();
        }
        
    }
}
