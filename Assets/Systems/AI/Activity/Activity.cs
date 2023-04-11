using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity.Brain
{
    [System.Serializable]
    public class Activity
    {
        public ActivityType _activityType;
        public Vector3Int GridCoord;
    }
}
