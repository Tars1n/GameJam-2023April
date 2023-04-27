using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    public class CustomDialogueStorage : MonoBehaviour
    {
        [SerializeReference] private List<DialoguePieceClass> _fellInPit;
        public List<DialoguePieceClass> FellInPit => _fellInPit;

        public void FellInPitDialogue()
        {
            
        }
    }
}
