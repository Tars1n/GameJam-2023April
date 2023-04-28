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
        public new void FallInPit()
        {
            if (IsCurrentlyMoving == false)
            {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.PlayerFallIntoPit);
            }
            base.FallInPit();
            _dialogueManager.DoDialoguePlayerDies(_fallInPitDialogue);
        }
        public new void TriggerTrap()
        {
            // base.TriggerTrap();
            _dialogueManager.DoDialoguePlayerDies(_triggersTrap);
        }
    }
}
