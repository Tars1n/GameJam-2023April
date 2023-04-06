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
        }

        private bool TryMoveTowardsTarget()
        {

            return true;
        }
    }
}
