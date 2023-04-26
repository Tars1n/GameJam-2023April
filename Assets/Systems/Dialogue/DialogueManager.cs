using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GameJam.Entity;
using TMPro;

namespace GameJam.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeReference] private List<DialoguePieceClass> _startDialogue;
        [SerializeReference] private List<DialoguePieceClass> _currentDialogue;
        [SerializeField] private GameObject _dialogueInCanvas;
        [SerializeField] private TMP_Text _dialogueTMPText;

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
                Debug.Log($"continue");
                _continueDialogue?.Invoke();
            }
        }
        private void BeginDialogue()
        {
            if (_currentDialogue == null) Debug.Log($"current dialogue = null");
            if (_currentDialogue.Count == 0) Debug.Log($"current dialgue count = 0");
            if ((_currentDialogue == null) || (_currentDialogue.Count == 0)) return;
            _dialogueIndex = 0;
            _gameMasterSingleton.GameSuspended = true;
            _dialogueInCanvas.SetActive(true);
            NextDialoguePiece();
        }
        private void NextDialoguePiece()
        {
            _continueDialogue -= NextDialoguePiece;
            if (_dialogueIndex >= _currentDialogue.Count)
            {
                _gameMasterSingleton.GameSuspended = false;
                _dialogueInCanvas.SetActive(false);
                _dialgueComplete?.Invoke();
                return;
            }
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialoguePieceTextClass))
            {
                Debug.Log($"dialoguepiecetextclass");
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.DialogueSting);
                DialoguePieceTextClass dialoguePieceText = (DialoguePieceTextClass)_currentDialogue[_dialogueIndex];
                Debug.Log(dialoguePieceText.DialogueTextStr);
                _dialogueTMPText.text = dialoguePieceText.DialogueTextStr;
                _dialogueIndex ++;  
                _continueDialogue += NextDialoguePiece; 
                return;         
            }
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialoguePieceSpawnEntityClass))
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);
                DialoguePieceSpawnEntityClass dialogueSpawnEntity = (DialoguePieceSpawnEntityClass)_currentDialogue[_dialogueIndex];
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
