using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    public class DialogueHideTextBox : DialoguePieceClass
    {
        public override void DoPiece(DialogueManager dialogueManager)
        {
            if (_customAudioClip != null)
                SoundManager.Instance.PlaySound(_customAudioClip);
            dialogueManager.TryCloseDialogueBox();
            FinishPiece();
        }
    }
}
