using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;
using UnityEngine.SceneManagement;

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
        [SerializeField] private ScoreSO _scoreSO;
        public ScoreSO ScoreSO => _scoreSO;        
        
        
        private void Awake() {
            _turnManager = GetComponent<TurnManager>();
        }

        private void Start()
        {
            StartCoroutine(LateStart());
            IfFirstSceneResetScore();
        }
        private void IfFirstSceneResetScore()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                _scoreSO.GameStartResetScore();
            }
        }
        IEnumerator LateStart()
        {
            yield return new WaitForSeconds(1f);
            _turnManager.BeginPlay();
        }
        public void LevelComplete()
        {
            _scoreSO.LevelCompleteSetScore();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        public void LevelFailed()
        {
            _scoreSO.RestartLevelScore();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
    }
}
