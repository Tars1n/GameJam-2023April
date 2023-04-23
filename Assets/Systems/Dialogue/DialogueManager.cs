using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GameJam.Entity;

namespace GameJam.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private List<DialoguePieceSO> _startDialogue;
        [SerializeField] private List<DialoguePieceSO> _currentDialogue;

        private GameMasterSingleton _gameMasterSingleton;
        private EntityManager _entityManager;
        public Action _continueDialogue;
        public Action _dialgueComplete;
        private int _dialogueIndex;

        private void Start() {
            {
                _entityManager = GameMaster.Instance.ReferenceManager.EntityManager;
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
            
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialogueText))
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.DialogueSting);
                DialogueText dialogueText = (DialogueText)_currentDialogue[_dialogueIndex];
                Debug.Log(dialogueText.DialogueTextStr);
                _dialogueIndex ++;  
                _continueDialogue += NextDialoguePiece; 
                return;         
            }
            
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialogueSpawnEntity))            
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);
                DialogueSpawnEntity dialogueSpawnEntity = (DialogueSpawnEntity)_currentDialogue[_dialogueIndex];
                GameObject go = Instantiate(dialogueSpawnEntity.EntityPrefab);
                EntityBase eb = go.GetComponent<EntityBase>();
                eb.SetupEntity();
                _dialogueIndex ++;
                NextDialoguePiece();
                return;
            }
        }
        private void OnDisable()
        {
            _continueDialogue -= NextDialoguePiece;
        }
    }
}
