using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity.Brain
{
    [System.Serializable]
    public class BrainLoopActivities : BrainBase
    {
        [SerializeField] private List<Activity> _activitiesToLoop;
        public List<Activity> ActivitiesToLoop => _activitiesToLoop;
        private void Awake()
        {
            _activitiesToLoop = new List<Activity>();
        }
        public override void Think()
        {

        }
    }
}
