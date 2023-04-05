using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;

namespace GameJam
{
    [RequireComponent(typeof(EntityManager))]
    public class ReferenceManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        public Tilemap Tilemap => _tilemap;
        [SerializeField] private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;

        private void Awake() {
            _entityManager = GetComponent<EntityManager>();
        }        
        
    }
}
