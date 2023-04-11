using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity.Brain;

namespace GameJam.Entity
{
    public class EntityMonster : EntityBase
    {
        //monster brain
        [SerializeField] private MonsterBlueprint _monsterBlueprint;
        // something that represents goal.
        [SerializeField] private EntityBase _targetEntity;
        [SerializeField] private TileNode _targetNode;
        [SerializeField] private Brain _brain;

        public override void DoTurnAction()
        {
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

            if (_turnManager.DebugLog) {Debug.Log($"{this} imagines moving towards a goal.");}
            yield return new WaitForSeconds(_turnManager.DelayBetweenActions);
            CompletedTurn();
        }
    }
}
