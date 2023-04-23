using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;
using GameJam.Level.Scene;
using UnityEngine.SceneManagement;

namespace GameJam.Level
{
    [RequireComponent(typeof(TurnManager), typeof(SceneHandler))]
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private MapManager _mapManager;
        public MapManager MapManager => _mapManager;
        private TurnManager _turnManager;
        public TurnManager TurnManager => _turnManager;
        [SerializeField] private AudioLibrary _audioLibrary;
        public AudioLibrary AudioLibrary => _audioLibrary;
        [SerializeField] private GameObject _slimeDrop;
        public GameObject SlimeDrop => _slimeDrop;
        [SerializeField] private bool _recordSlimeTrails;
        public bool RecordSlimeTrails => _recordSlimeTrails;
        [SerializeField] private ScoreSO _scoreSO;
        public ScoreSO ScoreSO => _scoreSO;
        private SceneHandler _sceneHandler;
        
        
        private void Awake() {
            _turnManager = GetComponent<TurnManager>();
            _sceneHandler = GetComponent<SceneHandler>();
        }

        private void Start()
        {
            _mapManager.SetupMap();
            _turnManager.Initialize();
            _mapManager.SetupTriggerTiles();
            IfFirstSceneResetScore();
            StartCoroutine(LateStart());
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
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.NextLevel);
            _scoreSO.LevelCompleteSetScore();
            _sceneHandler.LoadNextLevel();
        }
        public void LevelFailed()
        {
            _scoreSO.RestartLevelScore();
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.FailedLevel);
            _sceneHandler.RestartLevel();
        }
        
    }
}
