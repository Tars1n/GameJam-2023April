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
            
            StopAllCoroutines();
            StartCoroutine(LoadScene(thisLevel));
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
            StartCoroutine(LoadScene(nextLevel));
        }

        IEnumerator LoadScene(int sceneIndex)
        {
            GameMaster.Instance.Initialize();
            yield return null;

            SceneManager.LoadScene(sceneIndex);
        }
    }
}
