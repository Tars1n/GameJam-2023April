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
            StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadNextLevel()
        {
            int nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
            if (SceneManager.sceneCountInBuildSettings >= nextLevel)
            {
                Debug.LogWarning($"There are no more Scenes to load beyond this one: {SceneManager.GetActiveScene().buildIndex}. Make sure to add new levels to the Build Settings.");
                return;
            }
            StopAllCoroutines();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
