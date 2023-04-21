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
            int thisLevel = SceneManager.GetActiveScene().buildIndex;

            GameMaster.Instance.EndScene();
            LoadScene(thisLevel);
        }

        public void LoadNextLevel()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCountInBuildSettings < nextLevel)
            {
                Debug.LogWarning($"There are no more Scenes to load beyond this one: {SceneManager.GetActiveScene().buildIndex}. There are currently {SceneManager.sceneCountInBuildSettings} scenes in Build settings, make sure to add new levels to the Build Settings.");
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
    }
}
