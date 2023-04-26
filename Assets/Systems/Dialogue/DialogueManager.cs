using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GameJam.Entity;
using TMPro;
using UnityEngine.UI;
using GameJam.Map;

namespace GameJam.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeReference] private List<DialoguePieceClass> _startDialogue;
        [SerializeReference] private List<DialoguePieceClass> _currentDialogue;
        [SerializeField] private GameObject _dialogueInCanvas;
        [SerializeField] private TMP_Text _dialogueTMPText;
        [SerializeField] private Image _characterPortrait;

        private GameMasterSingleton _gameMasterSingleton;
        private EntityManager _entityManager;
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        public Action _continueDialogue;        
        public Action _dialgueComplete;
        private int _dialogueIndex;        

        private void Start() {
            {
                _entityManager = GameMaster.Instance.ReferenceManager.EntityManager;
                _gameMasterSingleton = GameMaster.GetSingleton();
                _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
                _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
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
                DoDialogueText();
                return;
            }
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialoguePieceSpawnEntityClass))
            {
                DoSpawnEntity();
                return;
            }
            if (_currentDialogue[_dialogueIndex].GetType() == typeof(DialoguePieceHopEntityClass))
            {
                DoHopEntity();
                return;
            }
        }
        private void DoHopEntity()
        {
            DialoguePieceHopEntityClass dialogueHopEntity = (DialoguePieceHopEntityClass)_currentDialogue[_dialogueIndex];
            TileNode tileNode = _tileNodeManager.GetTileFromAxial(dialogueHopEntity.TileToHopTo);
            _mapInteractionManager.HopEntityToPosFunc(dialogueHopEntity.Entity, tileNode, dialogueHopEntity.DurationOfHop, dialogueHopEntity.SlamAtEnd);
            _dialogueIndex ++;
            NextDialoguePiece();
        }

        private void DoSpawnEntity()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);
            DialoguePieceSpawnEntityClass dialogueSpawnEntity = (DialoguePieceSpawnEntityClass)_currentDialogue[_dialogueIndex];
            GameObject go = Instantiate(dialogueSpawnEntity.EntityPrefab);
            EntityBase eb = go.GetComponent<EntityBase>();
            eb.SetupEntity();
            _dialogueIndex++;
            NextDialoguePiece();
        }

        private void DoDialogueText()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.DialogueSting);
            DialoguePieceTextClass dialoguePieceTextClass = (DialoguePieceTextClass)_currentDialogue[_dialogueIndex];
            
            ChangePortrait(dialoguePieceTextClass);
            _dialogueTMPText.text = dialoguePieceTextClass.DialogueTextStr;
            _dialogueIndex++;
            _continueDialogue += NextDialoguePiece;
        }

        private void ChangePortrait(DialoguePieceTextClass dialoguePieceTextClass)
        {
            if (dialoguePieceTextClass.CharacterTalking != null)
            {
                _characterPortrait.sprite = dialoguePieceTextClass.CharacterTalking;
            }
        }

        private void OnDisable()
        {
            _continueDialogue -= NextDialoguePiece;
        }
    }
}
