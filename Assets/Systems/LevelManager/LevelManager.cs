using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam
{
    [RequireComponent(typeof(TurnManager))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapManager _mapManager;
        public MapManager MapManager => _mapManager;
        private TurnManager _turnManager;
        public TurnManager TurnManager => _turnManager;
        
        private void Awake() {
            _turnManager = GetComponent<TurnManager>();
        }
        
    }
}
