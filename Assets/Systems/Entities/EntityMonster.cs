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
        private BrainBase _brain;
        
        protected override void Awake()
        {
            base.Awake();
            _brain = GetComponent<BrainBase>();
        }

        public override void RefreshAction()
        {
            base.RefreshAction();
            _brain.TelegraphNextTurn();
        }
        public override void DoTurnAction()
        {
            _brain?.Think();
        }

        
    }
}
