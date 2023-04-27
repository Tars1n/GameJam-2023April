using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity.Brain;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceSpawnEntityPathClass : DialoguePieceSpawnEntityClass
    {
        [SerializeField] private List<Activity> _activityList;
        public List<Activity> ActivityList => _activityList;
    }
}
