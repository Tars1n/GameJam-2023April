using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{   
    [System.Serializable]
    public abstract class DialoguePieceClass
    {
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        public abstract void DoPiece(DialogueManager dialogueManager);
    }
}
