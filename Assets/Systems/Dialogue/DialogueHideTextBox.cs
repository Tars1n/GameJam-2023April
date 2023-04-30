using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    public class DialogueHideTextBox : DialoguePieceClass
    {
        public override void DoPiece(DialogueManager dialogueManager)
        {
            dialogueManager.TryCloseDialogueBox();
            FinishPiece();
        }
    }
}
