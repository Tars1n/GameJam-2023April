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
        private ReferenceManager Ref => _ref ? _ref : _ref = GameMaster.Instance.ReferenceManager;
        private MapInteractionManager _mapInteractionManager;
        private MapInteractionManager Interaction => _mapInteractionManager ? _mapInteractionManager : _mapInteractionManager = Ref.MapInteractionManager;
        private TurnManager _turnManager;

        private void Awake()
        {
            _entityMonster = GetComponent<EntityMonster>();            
        }
        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _mapInteractionManager = Ref.MapInteractionManager;
            _turnManager = Ref.TurnManager;
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

            Ref.PathFindingManager.MapAllTileNodesToTarget(_targetNode.GridCoordinate);
            TileNode node = Ref.TileNodeManager.GetNodeFromCoords(_currentTileNode.WalkingPathDirection);
            if (node == null)
            {
                Debug.LogWarning($"{this} was seeking invalid TileNode.");
                return;
            }

            if (Interaction.TryToTakeAction(null, node) == false) //Entity attempts to do action
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
