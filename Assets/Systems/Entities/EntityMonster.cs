using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity
{
    public class EntityMonster : EntityBase
    {
        //monster brain
        [SerializeField] private MonsterBlueprint _monsterBlueprint;
        // something that represents goal.
        [SerializeField] private EntityBase _targetEntity;
        [SerializeField] private TileNode _targetNode;

        public override void DoTurnAction()
        {
            _targetNode = _targetEntity?.CurrentTileNode;
            
            _ref.PathFindingManager.MapAllTileNodesToTarget(_targetNode.GridCoordinate);
            TileNode node = _ref.TileNodeManager.GetNodeFromCoords(CurrentTileNode.WalkingPathDirection);
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
