using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = false;
        private ReferenceManager _ref;
        [SerializeField] private List<EntityCharacter> _playerCharacters;
        public List<EntityCharacter> PlayerCharacters => _playerCharacters;
        [SerializeField] private List<EntityMonster> _monsters;
        [SerializeField] private List<EntityTrap> _traps;
        [SerializeField] private Queue<EntityBase> _mapEntityQueue;
        [SerializeField] private Queue<EntityBase> _entitiesToDestroy;
        private Map.TileNodeManager _tileNodeManager;
        private Level.LevelManager _levelManager;

        private void Awake() {
            _playerCharacters = new List<EntityCharacter>();
            _monsters = new List<EntityMonster>();
            _traps = new List<EntityTrap>();
            _mapEntityQueue = new Queue<EntityBase>();
            _entitiesToDestroy = new Queue<EntityBase>();
        }

        public void Initialize()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _levelManager = _ref.LevelManager;
            _tileNodeManager = _ref.MapManager.TileNodeManager;
            SetupAllEntities();
        }

        public void SetupAllEntities()
        {
            EntityBase[] foundEntities = FindObjectsOfType<EntityBase>(true);
            if (_debugLogs) { Debug.Log($"Setting up {foundEntities.Length} Entites."); }
            foreach (EntityBase entity in foundEntities)
            {
                entity.SetupEntity();
            }
        }

        public void AddEntity(EntityBase entity)
        {
            if (entity.GetType() == typeof(EntityCharacter))
                _playerCharacters.Add((EntityCharacter)entity);
            if (entity.GetType() == typeof(EntityMonster))
                _monsters.Add((EntityMonster)entity);
            if (entity.GetType() == typeof(EntityTrap))
                _traps.Add((EntityTrap)entity);
        }

        public void RemoveEntity(EntityBase entity)
        {
            if (entity.GetType() == typeof(EntityCharacter))
                { _playerCharacters.Remove((EntityCharacter)entity); }
            if (entity.GetType() == typeof(EntityMonster))
                { _monsters.Remove((EntityMonster)entity); }
            if (entity.GetType() == typeof(EntityTrap))
                { _traps.Remove((EntityTrap)entity); }
        }

        public void DestroyEntity(EntityBase entity)
        {
            RemoveEntity(entity);
            _entitiesToDestroy.Enqueue(entity);
            entity.DoDestroy();
        }

        public void DestroyRemovedEntities()
        {
            while (_entitiesToDestroy.Count > 0)
            {
                EntityBase entity = _entitiesToDestroy.Dequeue();
                Destroy(entity.gameObject);
            }
        }

        private void RemapAllEntities()
        {
            if (_debugLogs) {Debug.Log("Remapping all entities.");}
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.ClearTileNode(); }
            foreach (EntityMonster entity in _monsters)
                { entity.ClearTileNode(); }
            foreach (EntityTrap entity in _traps)
                { entity.ClearTileNode(); }

            _tileNodeManager.ClearAllNodeEntityLists();
            
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.LinkToTileNode(null); }
            foreach (EntityMonster entity in _monsters)
                { entity.LinkToTileNode(null); }
            foreach (EntityTrap entity in _traps)
                { entity.LinkToTileNode(null); }
        }

        public void QueueActionForAllEntities()
        {
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.RefreshAction(); }

            foreach (EntityMonster entity in _monsters)
            {
                entity.RefreshAction();
            }
            foreach (EntityTrap entity in _traps)
            {
                entity.RefreshAction();
            }
        }

        public void EnqueueAllMapEntities()
        {
            foreach (EntityMonster entity in _monsters)
                { _mapEntityQueue.Enqueue(entity); }
            foreach (EntityTrap entity in _traps)
                { _mapEntityQueue.Enqueue(entity); }
        }

        public bool DoesPlayerStillHaveAction()
        {
            bool answer = false;
            foreach (EntityCharacter playerEntity in _playerCharacters)
            {
                if (playerEntity.HasActionReady)
                    answer = true;
            }
            return answer;
        }

        public EntityBase GetNextReadyMapEntity()
        {
            //Get next Monster or Trap entity from the action queue.
            if (_mapEntityQueue.Count == 0)
                return null;
            
            if (_mapEntityQueue.Peek() == null || _mapEntityQueue.Peek().enabled == false || _mapEntityQueue.Peek().HasActionReady == false)
            {
                //entity was not valid, remove from list and get next.
                _mapEntityQueue.Dequeue();
                return GetNextReadyMapEntity();
            }

            return _mapEntityQueue?.Dequeue();
        }    

        public void TEST_EndPlayerTurn()
        {
            //Simulated player taking their last action.
            Debug.Log("Forcing end of turn.");
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.DoTurnAction(); }
            GameMaster.Instance.ReferenceManager.TurnManager.ActionCompleted();
        }   
    }
}
