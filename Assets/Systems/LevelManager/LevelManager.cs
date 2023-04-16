using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;

namespace GameJam
{
    [RequireComponent(typeof(TurnManager))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapManager _mapManager;
        public MapManager MapManager => _mapManager;
        private TurnManager _turnManager;
        public TurnManager TurnManager => _turnManager;
        [SerializeField] private GameObject _slimeDrop;
        public GameObject SlimeDrop => _slimeDrop;
        [SerializeField] private bool _recordSlimeTrails;
        public bool RecordSlimeTrails => _recordSlimeTrails;
        
        
        private void Awake() {
            _turnManager = GetComponent<TurnManager>();
        }

        private void Start()
        {
            StartCoroutine(LateStart());
        }

        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(1f);
            _turnManager.BeginPlay();
        }
        
    }
}
