using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameJam.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        private string _startOfScoreText = "Relics Gathered: ";
        private ScoreSO _scoreSO;
        private void Start() 
        {
            _scoreSO = GameMaster.Instance.ReferenceManager.LevelManager.ScoreSO;
            DisplayScore();
        }
        
        public void DisplayScore()
        {
            _scoreText.text = _startOfScoreText + _scoreSO.RelicsGatheredCurrent;
        }
    }
}
