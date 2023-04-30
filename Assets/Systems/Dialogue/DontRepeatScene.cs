using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    public class DontRepeatScene : DialoguePieceClass
    {
        [SerializeField] private string _nameOfDialogue;
        public override void DoPiece(DialogueManager dialogueManager)
        {
            if (GameMaster.Instance.CutscenesWatched.Contains(_nameOfDialogue))
            {
                _ref.DialogueManager.FinishDialogue();
                return;
            }
            GameMaster.Instance.CutscenesWatched.Add(_nameOfDialogue);
            _ref.DialogueManager.AutoNext(0);
            
        }
    }
}
