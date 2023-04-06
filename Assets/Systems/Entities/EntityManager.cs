using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityManager : MonoBehaviour
    {
        private ReferenceManager _ref;
        [SerializeField] private List<EntityCharacter> _playerCharacters;
        [SerializeField] private List<EntityMonster> _monsters;
        [SerializeField] private List<EntityTrap> _traps;
        private Map.TileNodeManager _tileNodeManager;

        private void Awake() {
            _playerCharacters = new List<EntityCharacter>();
            _monsters = new List<EntityMonster>();
            _traps = new List<EntityTrap>();
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

        private void RemapAllEntities()
        {
            _tileNodeManager.ClearAllNodeEntities();
            
            foreach (EntityCharacter entity in _playerCharacters)
                { entity.LinkToTileNode(); }
            foreach (EntityMonster entity in _monsters)
                { entity.LinkToTileNode(); }
            foreach (EntityTrap entity in _traps)
                { entity.LinkToTileNode(); }
        }
        
    }
}
