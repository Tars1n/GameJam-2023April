using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.StartScreen
{
    public class NewGame : MonoBehaviour
    {
        [SerializeField] private ScoreSO _scoreSO;

        private void Start()
        {
            _scoreSO?.GameStartResetScore();
        }
        public void NewGamFunc()
        {
            Debug.Log($"button clicked");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
