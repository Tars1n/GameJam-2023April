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
        private KillAdjacentPlayer _killAdjacentPlayer;
        // something that represents goal.
        private BrainBase _brain;
        
        private void Awake()
        {
            _brain = GetComponent<BrainBase>();
            _killAdjacentPlayer = GetComponent<KillAdjacentPlayer>();
        }

        public override void RefreshAction()
        {
            base.RefreshAction();
            _brain.TelegraphNextTurn();
        }
        public override void DoTurnAction()
        {
            _killAdjacentPlayer?.KillIfCan();
            _brain?.Think();
        }        
    }
}
