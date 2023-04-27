using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityCharacter : EntityBase
    {    
        public override void DoTurnAction()
        {
            CompletedTurn();
        }

        public override void DoDestroy()
        {
            LeaveTileNode();
            if (_debugLog) { Debug.Log($"Player character died.");}

            this.gameObject.SetActive(false);
            // GameMaster.Instance.ReferenceManager.LevelManager.LevelFailed();
        }
    }
}
