using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using GameJam.Entity;
using TMPro;
using UnityEngine.UI;
using GameJam.Map;
using GameJam.Entity.Brain;
using GameJam.Level;

namespace GameJam.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        private ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeReference] private List<DialoguePieceClass> _startDialogue;
        [SerializeReference] private List<DialoguePieceClass> _endDialogue;
        [SerializeReference] private List<DialoguePieceClass> _currentDialogue;
        [SerializeField] private GameObject _dialogueInCanvas;
        [SerializeField] private TMP_Text _dialogueTMPText;
        [SerializeField] private Image _characterPortrait;

        private GameMasterSingleton _gameMasterSingleton;
        private EntityManager _entityManager;
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        private LevelManager _levelManager;
        public Action OnContinueDialogue;        
        public Action OnDialogueComplete;
        private int _dialogueIndex; 
        private bool _levelLost;       

        private void Start() {
            {
                _entityManager = GameMaster.Instance.ReferenceManager.EntityManager;
                _gameMasterSingleton = GameMaster.GetSingleton();
                _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
                _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
                _levelManager = GameMaster.Instance.ReferenceManager.LevelManager;              
            }
        }

        public void DoLevelStartDialogue()
        {
            DoDialogue(_startDialogue);
        }

        public void DoDialogue(List<DialoguePieceClass> dialogue)
        {
            _currentDialogue = dialogue;
            BeginDialogue();
        }
        public void DoEndDialogue()
        {
            _currentDialogue = _endDialogue;
            BeginDialogue();
        }
        public void DoDialoguePlayerDies(List<DialoguePieceClass> killDialogue)
        {
            _currentDialogue = killDialogue;
            _levelLost = true;
            BeginDialogue();
        }
        private void Update()
        {
            if (GameMaster.Instance.InCutscene == false) return;
            if ((_gameMasterSingleton.GameSuspended) && (Mouse.current.leftButton.wasPressedThisFrame))
            {
                Debug.Log($"continue");
                // OnContinueDialogue?.Invoke();
                NextDialoguePiece();
            }
        }
        private void BeginDialogue()
        {
            // if (_currentDialogue == null) Debug.Log($"current dialogue = null");
            // if (_currentDialogue.Count == 0) Debug.Log($"current dialgue count = 0");
            // if ((_currentDialogue == null) || (_currentDialogue.Count == 0)) return;
            _dialogueIndex = 0;
            GameMaster.Instance.InCutscene = true;
            GameMaster.Instance.GameSuspended = true;
            GameMaster.Instance.TilemapInteractable = false;
            
            NextDialoguePiece();
        }

        public void NextDialoguePiece()
        {
            OnContinueDialogue -= NextDialoguePiece;
            if (_dialogueIndex >= _currentDialogue.Count)
            {
                DialogueDoneCheckIfLevelEnd();
                DialogueDoneCheckIfLevelLost();
                FinishDialogue();
                return;
            }
            if (_currentDialogue[_dialogueIndex] == null)
            {
                _dialogueIndex ++;
                NextDialoguePiece();
                return;
            }

            _currentDialogue[_dialogueIndex].DoPiece(this);
            _dialogueIndex ++;
            return;
        }

        public void TryOpenDialogueBox()
        {
            _dialogueInCanvas.SetActive(true);
        }

        public void TryCloseDialogueBox()
        {
            _dialogueInCanvas.SetActive(false);
        }
        
        private void FinishDialogue()
        {
            GameMaster.Instance.GameSuspended = false;
            GameMaster.Instance.InCutscene = false;
            _dialogueInCanvas.SetActive(false);
            _ref.TurnManager.BeginPlay();

            OnDialogueComplete?.Invoke();
        }

        private void DialogueDoneCheckIfLevelEnd()
        {
            if (_currentDialogue == _endDialogue)
            {
                _levelManager.LevelComplete();
            }
        }
        private void DialogueDoneCheckIfLevelLost()
        {
            if (_levelLost)
            {
                _levelManager.LevelFailed();
            }
        }

        private void DoDialogueText()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.DialogueSting);
            DialoguePieceTextClass dialoguePieceTextClass = (DialoguePieceTextClass)_currentDialogue[_dialogueIndex];
            
            ChangePortrait(dialoguePieceTextClass);
            _dialogueTMPText.text = dialoguePieceTextClass.DialogueTextStr;
            _dialogueIndex++;
            OnContinueDialogue += NextDialoguePiece;
        }

        public void SetText(string text)
        {
            _dialogueTMPText.text = text;
        }

        public void ChangePortrait(DialoguePieceTextClass dialoguePieceTextClass)
        {
            if (dialoguePieceTextClass.CharacterTalking != null)
            {
                _characterPortrait.sprite = dialoguePieceTextClass.CharacterTalking;
                // StrechPortraitScale(dialoguePieceTextClass);
            }
        }
        public void ChangePortrait(Sprite sprite)
        {
            if (sprite != null)
            {
                _characterPortrait.sprite = sprite;
            }
        }

        // private void StrechPortraitScale(DialoguePieceTextClass dialoguePieceTextClass)
        // {
        //     RectTransform rectTransform = _characterPortrait.GetComponent<RectTransform>();
        //     float rectXScale = rectTransform.localScale.x;
        //     float newRectYScale = (dialoguePieceTextClass.CharacterTalking.bounds.max.y - dialoguePieceTextClass.CharacterTalking.bounds.min.y) / (dialoguePieceTextClass.CharacterTalking.bounds.max.x - dialoguePieceTextClass.CharacterTalking.bounds.min.x);
        //     newRectYScale *= rectXScale;
        //     rectTransform.localScale = new Vector2(rectXScale, newRectYScale);
        // }

        private void OnDisable()
        {
            OnContinueDialogue -= NextDialoguePiece;
        }
    }
}
