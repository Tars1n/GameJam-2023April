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
            if (_targetEntity == null || _targetNode == null)
            {
                // Debug.Log($"{this.gameObject} has no valid target to seek. Ending turn.");
                { StartCoroutine(TakeNoAction()); }
                return;
            }

            _ref.PathFindingManager.MapAllTileNodesToTarget(_targetNode.GridCoordinate);
            TileNode node = _ref.TileNodeManager.GetNodeFromCoords(_currentTileNode.WalkingPathDirection);

            if (_mapInteractionManager.TryToTakeAction(null, node) == false) //Entity attempts to do action
            { StartCoroutine(TakeNoAction()); }
        }

        public override void TelegraphNextTurn()
        {

        }

        IEnumerator TakeNoAction()
        {
            if (_turnManager.DebugLog) { Debug.Log($"{this} stands in place."); }
            yield return new WaitForSeconds(_turnManager.DelayBetweenActions);
            _entityMonster.ActionCompleted();
        }
    }
}
