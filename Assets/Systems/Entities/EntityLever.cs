using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;

namespace GameJam.Entity
{
    public class EntityLever : EntityBase
    {
        private LeverTrigger _leverTrigger;

        private void Awake()
        {
            _leverTrigger = GetComponent<LeverTrigger>();
        }
        public override void DoTurnAction()
        {
            //Lever does not take a turn.
        }
        public override void RefreshAction()
        {
            if (this.gameObject.activeInHierarchy == false)
                { return; }
            _hasActionReady = false; //prevents from taking time in the turn order.
            _leverTrigger.ActivatedThisTurn = false;
        }

        public override void CollidedWithObject()
        {
            _leverTrigger.ToggleLever();
        }
    }
}
