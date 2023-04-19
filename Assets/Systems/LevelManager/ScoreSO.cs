using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    using UnityEngine;
    
    [System.Serializable]
    [CreateAssetMenu (fileName = "ScoreSOInstance" , menuName = "NewScoreSO")]
    public class ScoreSO : ScriptableObject
    {
        [SerializeField] private int _relicsGatheredCurrent;
        public int RelicsGatheredCurrent => _relicsGatheredCurrent;
        [SerializeField] private int _relicsGatheredLevelComplete;
        public int RelicsGatheredLevelComplete => _relicsGatheredLevelComplete;

        public void GameStartResetScore()
        {
            _relicsGatheredCurrent = 0;
            _relicsGatheredLevelComplete = 0;
        }
        public void AddRelics(int amount)
        {
            _relicsGatheredCurrent += amount;
        }
        public void LevelCompleteSetScore()
        {
            _relicsGatheredLevelComplete = _relicsGatheredCurrent;
        }
        public void RestartLevelScore()
        {
            _relicsGatheredCurrent = _relicsGatheredLevelComplete;
        }
    }
}
