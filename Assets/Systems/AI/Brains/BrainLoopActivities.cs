using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;

namespace GameJam.Entity.Brain
{
    [System.Serializable]
    public class BrainLoopActivities : BrainBase
    {
        [SerializeField] private List<Activity> _activitiesToLoop;
        public List<Activity> ActivitiesToLoop => _activitiesToLoop;
        [SerializeField] private int _stepInActivityLoop;
        private MapManager _mapManager;
        private ReferenceManager _ref;
        private EntityBase _entityBase;
        private void Awake()
        {
            _activitiesToLoop = new List<Activity>();     
            _entityBase = GetComponent<EntityBase>();      
        }
        private void Start() 
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _mapManager = _ref.MapManager;
        }
        public override void Think()
        {
            Vector3Int axialToMoveTo = _mapManager.CastOddRowToAxial(_entityBase.CurrentTileNode.GridCoordinate);
            axialToMoveTo += _activitiesToLoop[_stepInActivityLoop].GridCoord;
            Debug.Log($"move to coords " + axialToMoveTo);
            IncreaseStep();
        }
        private void IncreaseStep()
        {
            _stepInActivityLoop ++;
            if (_stepInActivityLoop >= _activitiesToLoop.Count)
            {
                _stepInActivityLoop = 0;
            }
        }
    }
}
