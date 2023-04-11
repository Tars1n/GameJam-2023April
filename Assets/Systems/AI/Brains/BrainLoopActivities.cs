using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity.Brain
{
    public class BrainLoopActivities : BrainBase
    {
        [SerializeField] private List<Activity> _activitiesToLoop;
        public List<Activity> ActivitiesToLoop => _activitiesToLoop;
    }
}
