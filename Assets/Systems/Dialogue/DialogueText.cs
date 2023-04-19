using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    [System.Serializable]
    [CreateAssetMenu (fileName = "DialogueText" , menuName = "Dialogue/Text")]
    public class DialogueText : DialoguePieceSO
    {
        [SerializeField] private string _dialogueText;
        public string DialogueTextStr => _dialogueText;
    }
}
