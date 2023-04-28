using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Level;
using GameJam.Dialogue;

namespace GameJam.Map.TriggerTiles
{
    public class LevelCompletion : TriggerTileManager
    {
        private LevelManager _levelManager;
        private RelicPickup _relicPickup;
        private DialogueManager _dialogueManager;
        private int _relicsRequiredForCompletion;
        private bool _levelComplete;

        private void Awake()
        {
            _levelManager = GetComponent<LevelManager>();
            _relicPickup = GetComponent<RelicPickup>();
        }
        protected override void Start() 
        {
            base.Start();
            _dialogueManager = GameMaster.Instance.ReferenceManager.DialogueManager;
        }
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase.GetType() != typeof(EntityCharacter)) return;
            if (_relicsRequiredForCompletion > _relicPickup.RelicsGathered) return;
            if (_triggerLocationTiles == null) return;
            if (entityBase.GetType() != typeof(EntityCharacter)) return;
            if (tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tileNode}");
                return;
            }
            _levelComplete = true;
        }
        public void CheckIfLevelComplete()
        {
            if (_levelComplete)
            {
                _dialogueManager.DoEndDialogue();
                // _levelManager.LevelComplete();
            }
        }
    }
}
