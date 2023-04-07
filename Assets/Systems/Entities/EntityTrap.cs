using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityTrap : EntityBase
    {
        [SerializeField] private TrapBlueprint _trapBlueprint;
        
        public override void DoTurnAction()
        {
            StartCoroutine(TickTrap());
        }

        IEnumerator TickTrap()
        {
            if (_turnManager.DebugLog) {Debug.Log("Trap makes ticking noises");}
            yield return new WaitForSeconds(_turnManager.DelayBetweenActions);
            CompletedTurn();
        }
    }
}
