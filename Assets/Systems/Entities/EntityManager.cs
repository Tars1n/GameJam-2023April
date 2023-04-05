using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField] private List<EntityCharacter> _playerCharacters;
        [SerializeField] private List<EntityMonster> _monsters;
        [SerializeField] private List<EntityTrap> _traps;

        private void Awake() {
            _playerCharacters = new List<EntityCharacter>();
            _monsters = new List<EntityMonster>();
            _traps = new List<EntityTrap>();
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
    }
}
