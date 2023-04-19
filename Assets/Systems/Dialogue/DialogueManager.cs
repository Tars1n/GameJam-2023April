using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace GameJam.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private List<DialoguePieceSO> _startDialogue;
        [SerializeField] private List<DialoguePieceSO> _currentDialogue;

        private GameMasterSingleton _gameMasterSingleton;
        public Action _continueDialogue;
        public Action _dialgueComplete;
        private int _dialogueIndex;

        private void Start() {
            {
                _gameMasterSingleton = GameMaster.GetSingleton();
                _currentDialogue = _startDialogue; 
                BeginDialogue();               
            }
        }
        private void Update()
        {
            if ((_gameMasterSingleton.GameSuspended) && (Mouse.current.leftButton.wasPressedThisFrame))
            {
                _continueDialogue?.Invoke();
            }
        }
        private void BeginDialogue()
        {
            _dialogueIndex = 0;
            if ((_currentDialogue == null) || (_currentDialogue.Count == 0)) return;
            _gameMasterSingleton.GameSuspended = true;
            NextDialoguePiece();
        }
        private void NextDialoguePiece()
        {
            _continueDialogue -= NextDialoguePiece;
            if (_dialogueIndex >= _currentDialogue.Count)
            {
                _gameMasterSingleton.GameSuspended = false;
                _dialgueComplete?.Invoke();
                return;
            }
            DialogueText dialogueText = (DialogueText)_currentDialogue[_dialogueIndex];
            if (dialogueText != null)
            {
                Debug.Log(dialogueText.DialogueTextStr);
                _dialogueIndex ++;  
                _continueDialogue += NextDialoguePiece;          
            }
        }
        private void OnDisable()
        {
            _continueDialogue -= NextDialoguePiece;
        }
    }
}
