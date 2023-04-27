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
        private ReferenceManager _ref => GameMaster.Instance.ReferenceManager;

        private void Awake()
        {
            _entityMonster = GetComponent<EntityMonster>();            
        }
     
        public override void Think()
        {
            TileNode _currentTileNode = _entityMonster.CurrentTileNode;
            _targetNode = _targetEntity?.CurrentTileNode;
            if (_targetEntity == null || _targetNode == null)
            {
                // Debug.Log($"{this.gameObject} has no valid target to seek. Ending turn.");
                TakeNoAction();
                return;
            }

            _ref.PathFindingManager.MapAllTileNodesToTarget(_targetNode.GridCoordinate);
            TileNode node = _ref.TileNodeManager.GetNodeFromCoords(_currentTileNode.WalkingPathDirection);
            if (node == null)
            {
                Debug.LogWarning($"{this} was seeking invalid TileNode.");
                return;
            }

            if (_ref.MapInteractionManager.TryToTakeAction(null, node) == false) //Entity attempts to do action
                { TakeNoAction(); }
        }

        public override void TelegraphNextTurn()
        {

        }

        private void TakeNoAction()
        {
            _ref.MapInteractionManager.HopEntity(_entityMonster, _entityMonster.CurrentTileNode, 0);
            // if (_ref.TurnManager.DebugLog) { Debug.Log($"{this} stands in place."); }
            // yield return new WaitForSeconds(_ref.TurnManager.DelayBetweenActions);
            // _entityMonster.ActionCompleted();
        }
        public void SetTargetEntity(EntityBase targetEntity)
        {
            _targetEntity = targetEntity;
        }
    }
}
