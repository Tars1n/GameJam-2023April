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
        [SerializeField] private TileNode _targetNode;

        public override void DoTurnAction()
        {
            TryMoveTowardsTarget();
            HasActedThisRound = true;
        }

        private bool TryMoveTowardsTarget()
        {
            //_ref.PlotPath. (_currentTileNode.GridPosition, _targetNode);
            return true;
        }
    }
}
