using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityManager : MonoBehaviour
    {
        private ReferenceManager _ref;
        [SerializeField] private List<EntityCharacter> _playerCharacters;
        public List<EntityCharacter> PlayerCharacters => _playerCharacters;
        [SerializeField] private List<EntityMonster> _monsters;
        [SerializeField] private List<EntityTrap> _traps;
        [SerializeField] private Queue<EntityBase> _mapEntityQueue;
        private Map.TileNodeManager _tileNodeManager;

        private void Awake() {
            _playerCharacters = new List<EntityCharacter>();
            _monsters = new List<EntityMonster>();
            _traps = new List<EntityTrap>();
            _mapEntityQueue = new Queue<EntityBase>();
        }

        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _tileNodeManager = _ref.MapManager.TileNodeManager;
            RemapAllEntities(); 
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

        public bool TryRemoveEntity(EntityBase entity)
        {
            bool removed = false;
            if (entity.GetType() == typeof(EntityCharacter))
            {
                _playerCharacters.Remove((EntityCharacter)entity);
                removed = true;
            }
            if (entity.GetType() == typeof(EntityMonster))
            {
                _monsters.Add((EntityMonster)entity);
                removed = true;
            }
            if (entity.GetType() == typeof(EntityTrap))
            {
                _traps.Add((EntityTrap)entity);
                removed = true;
            }
            return removed;
        }

        private void RemapAllEntities()
        {
            _tileNodeManager.ClearAllNodeEntities();
            
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
                { entity.HasActionReady = true; }

            foreach (EntityMonster entity in _monsters)
            {
                entity.HasActionReady = true;
                _mapEntityQueue.Enqueue(entity);
            }
            foreach (EntityTrap entity in _traps)
            {
                entity.HasActionReady = true;
                _mapEntityQueue.Enqueue(entity);
            }
        }

        private void EnqueueAllMapEntities()
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

            return _mapEntityQueue?.Dequeue();
        }    

        public void TEST_EndPlayerTurn()
        {
            //Simulated player taking their last action.
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.HasActionReady = false; }
            GameMaster.Instance.ReferenceManager.TurnManager.ActionCompleted();
        }   
    }
}
