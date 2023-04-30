using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;

namespace GameJam.Entity
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = false;
        private ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] private List<EntityCharacter> _playerCharacters;
        public List<EntityCharacter> PlayerCharacters => _playerCharacters;
        [SerializeField] private List<EntityMonster> _monsters;
        [SerializeField] private List<EntityTrap> _traps;
        [SerializeField] private List<EntityLever> _levers;
        [SerializeField] private Queue<EntityBase> _mapEntityQueue;
        [SerializeField] private Queue<EntityBase> _entitiesToDestroy;
        private Map.TileNodeManager _tileNodeManager;
        private Level.LevelManager _levelManager;

        private void Awake() {
            _playerCharacters = new List<EntityCharacter>();
            _monsters = new List<EntityMonster>();
            _traps = new List<EntityTrap>();
            _levers = new List<EntityLever>();
            _mapEntityQueue = new Queue<EntityBase>();
            _entitiesToDestroy = new Queue<EntityBase>();
        }

        public void Initialize()
        {
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
                if (!entity.gameObject.activeInHierarchy) continue; //?Notice the true flag at the top, that was for including inactive entities.
                entity.SetupEntity();
            }
        }

        public EntityBase SpawnEntity(GameObject entity, Vector3Int coords)
        {
            if (entity == null) return null;
            Vector3 spawnPos = _ref.MapManager.Map.CellToWorld(coords);
            GameObject go = Instantiate(entity, spawnPos, Quaternion.identity);
            EntityBase eb = go.GetComponent<EntityBase>();
            eb.SetupEntity();
            eb.RefreshAction();
            return eb;
        }

        public TriggerTileManager SpawnTriggerObject(GameObject triggerObject, Vector3Int coords)
        {
            EntityBase eb = SpawnEntity(triggerObject, coords);
            if (eb == null) return null;
            
            TriggerTileManager ttm = eb.GetComponent<TriggerTileManager>();
            ttm.SetupTriggerTiles();
            return ttm;
        }

        public void AddEntity(EntityBase entity)
        {
            if (entity is EntityCharacter)
                _playerCharacters.Add((EntityCharacter)entity);
            if (entity is EntityMonster)
                _monsters.Add((EntityMonster)entity);
            if (entity is EntityTrap)
                _traps.Add((EntityTrap)entity);
            if (entity is EntityLever)
                _levers.Add((EntityLever)entity);
        }

        public void RemoveEntityReference(EntityBase entity)
        {
            if (entity == null) return;
            if (entity is EntityCharacter)
                { _playerCharacters.Remove((EntityCharacter)entity); }
            if (entity is EntityMonster)
                { _monsters.Remove((EntityMonster)entity); }
            if (entity is EntityTrap)
                { _traps.Remove((EntityTrap)entity); }
            if (entity is EntityLever)
                _levers.Remove((EntityLever)entity);
            if (_debugLogs)
                Debug.Log($"{entity} removed from EntityManager.");
        }

        public void DestroyEntity(EntityBase entity)
        {
            if (entity == null)
            {
                Debug.LogWarning($"Destroy Entity called on non-entity.");
                return;
            }
            RemoveEntityReference(entity);
            _entitiesToDestroy.Enqueue(entity);
            entity.DoDestroy();
        }

        public void DestroyRemovedEntities()
        {
            while (_entitiesToDestroy.Count > 0)
            {
                EntityBase entity = _entitiesToDestroy.Dequeue();
                if (_debugLogs)
                    Debug.Log($"EntityManager destroyed GameObject: {entity}");
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
            foreach (EntityLever entity in _levers)
                { entity.ClearTileNode(); }

            _tileNodeManager.ClearAllNodeEntityLists();
            
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.LinkToTileNode(null); }
            foreach (EntityMonster entity in _monsters)
                { entity.LinkToTileNode(null); }
            foreach (EntityTrap entity in _traps)
                { entity.LinkToTileNode(null); }
            foreach (EntityLever entity in _levers)
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
            foreach (EntityLever entity in _levers)
            {
                entity.RefreshAction();
            }
        }

        public void EnqueueAllMapEntities()
        {
            foreach (EntityMonster entity in _monsters)
                { _mapEntityQueue.Enqueue(entity); }
            // foreach (EntityTrap entity in _traps)
                // { _mapEntityQueue.Enqueue(entity); }
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

        public EntityCharacter GetNextActivePlayerCharacter()
        {
            foreach (EntityCharacter playerEntity in _playerCharacters)
            {
                if (playerEntity.HasActionReady)
                    return playerEntity;
            }
            return null;
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
