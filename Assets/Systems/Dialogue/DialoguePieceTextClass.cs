using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceTextClass : DialoguePieceClass
    {
        [TextArea(5,20)]
        [SerializeField] private string _dialogueText;
        public string DialogueTextStr => _dialogueText;
        [SerializeField] private Sprite _characterTalking;
        public Sprite CharacterTalking => _characterTalking;

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoDialogueText(dialogueManager);
        }

        private void DoDialogueText(DialogueManager dialogueManager)
        {
            dialogueManager.TryOpenDialogueBox();
            if (_customAudioClip != null)
                SoundManager.Instance.PlaySound(_customAudioClip);
            else
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.DialogueSting);
            
            dialogueManager.ChangePortrait(_characterTalking);
            dialogueManager.SetText(_dialogueText);
            dialogueManager.WaitOnClick = true;
        }
    }
}
