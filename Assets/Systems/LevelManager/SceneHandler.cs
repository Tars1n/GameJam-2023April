using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.Level.Scene
{
    public class SceneHandler : MonoBehaviour
    {
        public void RestartLevel()
        {
            //int thisLevel = SceneManager.GetActiveScene().buildIndex;
            string thisLevel = SceneManager.GetActiveScene().name;

            GameMaster.Instance.EndScene();
            LoadScene(thisLevel);
        }

        public void LoadNextLevel()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCountInBuildSettings <= nextLevel)
            {
                Debug.LogWarning($"There are no more Scenes to load beyond this one: {SceneManager.GetActiveScene().buildIndex}. Make sure to add new levels in File->Build Settings.");
                return;
            }
            
            GameMaster.Instance.EndScene();
            LoadScene(nextLevel);
        }

        private void LoadScene(int sceneIndex)
        {
            GameMaster.Instance.Initialize();
            SceneManager.LoadScene(sceneIndex);
        }

        private void LoadScene(string sceneName)
        {
            GameMaster.Instance.Initialize();
            SceneManager.LoadScene(sceneName);
        }
    }
}
