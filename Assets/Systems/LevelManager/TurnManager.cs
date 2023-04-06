using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam
{
    [RequireComponent(typeof(EntityManager))]
    public class TurnManager : MonoBehaviour
    {
        private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;
        

        private void Awake()
        {
            _entityManager = GetComponent<EntityManager>();    
        }
        private void StartPlayerTurn()
        {
            
        }
    }
}
