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
            base.DoDestroy();
            Debug.Log($"Player character died.");
        }
        public override void FallInPit()
        {
            base.FallInPit();
            if (IsCurrentlyMoving == false)
            {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.PlayerFallIntoPit);
            }
            _dialogueManager.DoDialoguePlayerDies(_fallInPitDialogue);
        }
        public override void TriggerTrap()
        {
            base.TriggerTrap();
            _dialogueManager.DoDialoguePlayerDies(_triggersTrap);
        }
    }
}
