using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam
{
    public class GameMaster
    {     
        private static GameMasterSingleton _instance;
        public static GameMasterSingleton Instance { get { return GetSingleton(); }}

        public static GameMasterSingleton GetSingleton()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject();
                go.name = "GameMaster";
                _instance = go.AddComponent<GameMasterSingleton>();
            }
            return _instance;
        }
    }
}
