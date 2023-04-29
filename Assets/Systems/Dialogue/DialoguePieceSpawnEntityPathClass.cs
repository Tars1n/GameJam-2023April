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

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnEntity();
            SetActivities();
        }

        private void SetActivities()
        {
            if ((_activityList == null) || (_activityList.Count == 0)) return;            
            BrainLoopActivities brainLoop = _entitySpawned.GetComponent<BrainLoopActivities>();
            if (brainLoop == null) return;
            brainLoop.SetActivitiesToLoop(_activityList);
            
        }
    }
}
