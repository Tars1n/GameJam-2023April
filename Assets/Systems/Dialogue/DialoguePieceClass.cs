using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace GameJam.Dialogue
{   
    [System.Serializable]
    public abstract class DialoguePieceClass
    {
        protected ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] protected AudioClip _customAudioClip;
        [SerializeField] protected bool _skipClick;
        [ShowIf("_skipClick"), SerializeField] protected float _waitForSeconds = 0f;

        public abstract void DoPiece(DialogueManager dialogueManager);

        protected virtual void FinishPiece()
        {
            if (_skipClick)
            {
                _ref.DialogueManager.AutoNext(_waitForSeconds);
                return;
            }
            _ref.DialogueManager.WaitOnClick = true;
        }

        IEnumerator DoRunNext()
        {
            yield return new WaitForSeconds(_waitForSeconds);
            
        }

    }
}
