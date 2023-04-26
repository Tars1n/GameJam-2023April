using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceTextClass : DialoguePieceClass
    {
        [SerializeField] private string _dialogueText;
        public string DialogueTextStr => _dialogueText;
        [SerializeField] private Sprite _characterTalking;
        public Sprite CharacterTalking => _characterTalking;
    }
}
