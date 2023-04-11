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
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        private void Awake()
        {  
            _entityBase = GetComponent<EntityBase>();      
        }
        private void Start() 
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _mapManager = _ref.MapManager;
            _mapInteractionManager = _ref.MapInteractionManager;
            _tileNodeManager = _ref.TileNodeManager;
        }
        public override void Think()
        {
            Vector3Int axialToMoveTo = _mapManager.CastOddRowToAxial(_entityBase.CurrentTileNode.GridCoordinate);
            axialToMoveTo += _activitiesToLoop[_stepInActivityLoop].GridCoord;
            Debug.Log($"move to coords " + axialToMoveTo);
            _mapInteractionManager.MoveEntity(_entityBase, _tileNodeManager.GetNodeFromCoords(_mapManager.CastAxialToOddRow(axialToMoveTo)));
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
