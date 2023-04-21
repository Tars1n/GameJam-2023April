using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;

namespace GameJam.Entity
{
    public class EntityLever : EntityBase
    {
        private LeverTrigger _leverTrigger;

        protected override void Awake()
        {
            base.Awake();
            _leverTrigger = GetComponent<LeverTrigger>();
        }
        public override void DoTurnAction()
        {
            //Lever does not take a turn.
        }

        public override void CollidedWithObject()
        {
            _leverTrigger.ToggleLever();
        }
    }
}
