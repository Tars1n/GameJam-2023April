using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameJam.StartScreen
{
    public class NewGame : MonoBehaviour
    {
        public void NewGamFunc()
        {
            Debug.Log($"button clicked");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
