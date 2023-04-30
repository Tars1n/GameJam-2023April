using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;

namespace GameJam.Dialogue
{
    public class DialogueSpawnTrap : DialoguePieceClass
    {
        [SerializeField] protected GameObject _trapPrefab;
        public GameObject TrapPrefab => _trapPrefab;
        [SerializeField] protected Vector3Int _coords;
        protected SingleTileTrap _trapSpawned;
        public SingleTileTrap TrapSpawned => _trapSpawned;
        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnTrap();
            _ref.DialogueManager.NextDialoguePiece();
        }

        protected virtual void DoSpawnTrap()
        {
            if (_customAudioClip != null)
                SoundManager.Instance.PlaySound(_customAudioClip);
            else
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);
            // DialoguePieceSpawnEntityClass dialogueSpawnEntity = (DialoguePieceSpawnEntityClass)_currentDialogue[_dialogueIndex];
            TriggerTileManager ttm = _ref.EntityManager.SpawnTriggerObject(_trapPrefab, _coords);
            if (ttm is SingleTileTrap)
            _trapSpawned = (SingleTileTrap)ttm;
            if (_trapSpawned == null)
            {
                Debug.LogWarning("Trap failed to spawn.");
                return;
            }
            Debug.Log($"Successfully spawned {_trapSpawned}");
        }
        
    }
}
