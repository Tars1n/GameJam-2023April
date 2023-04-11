using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;


namespace GameJam.Entity.Brain
{
    public class BrainSeekTarget : BrainBase
    {
        private EntityMonster _entityMonster;
        [SerializeField] private EntityBase _targetEntity;
        [SerializeField] private TileNode _targetNode;
        private ReferenceManager _ref;
        private MapInteractionManager _mapInteractionManager;
        private TurnManager _turnManager;

        private void Awake()
        {
            _entityMonster = GetComponent<EntityMonster>();            
        }
        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _mapInteractionManager = _ref.MapInteractionManager;
            _turnManager = _ref.TurnManager;
        }
        public override void Think()
        {
            TileNode _currentTileNode = _entityMonster.CurrentTileNode;
            _targetNode = _targetEntity?.CurrentTileNode;

            _ref.PathFindingManager.MapAllTileNodesToTarget(_targetNode.GridCoordinate);
            //Debug.Log($"targeting {_targetEntity} at coordinate {_targetNode.GridCoordinate}");
            TileNode node = _ref.TileNodeManager.GetNodeFromCoords(_currentTileNode.WalkingPathDirection);
            //Debug.Log($"standing on {_currentTileNode.GridCoordinate} trying to move to {_currentTileNode.WalkingPathDirection}.");
            if (_mapInteractionManager.TryToTakeAction(node) == false)
            { StartCoroutine(TryMoveTowardsTarget()); }
        }

        IEnumerator TryMoveTowardsTarget()
        {
            //_ref.PlotPath. (_currentTileNode.GridPosition, _targetNode);

            if (_turnManager.DebugLog) { Debug.Log($"{this} imagines moving towards a goal."); }
            yield return new WaitForSeconds(_turnManager.DelayBetweenActions);
            _entityMonster.ActionCompleted();
        }
    }
}
