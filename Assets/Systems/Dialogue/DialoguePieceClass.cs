using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{   
    [System.Serializable]
    public abstract class DialoguePieceClass
    {
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] protected AudioClip _customAudioClip;
        [SerializeField] protected bool _skipClick;
        public abstract void DoPiece(DialogueManager dialogueManager);

        protected virtual void FinishPiece()
        {
            if (_skipClick)
            {
                _ref.DialogueManager.NextDialoguePiece();
                return;
            }
            _ref.DialogueManager.WaitOnClick = true;
        }

    }
}
