using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Dialogue;

namespace GameJam
{
    public class DialogueBeginLevel : DialoguePieceClass
    {
        public override void DoPiece(DialogueManager dialogueManager)
        {
            GameMaster.Instance.ReferenceManager.TurnManager.BeginPlay();
        }
    }
}
