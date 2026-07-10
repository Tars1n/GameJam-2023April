using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;
using UnityEngine.LowLevel;

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
            FinishPiece();
            
        }

        protected virtual void DoSpawnTrap()
        {
            if (_customAudioClip == null)
                _customAudioClip = SoundManager.Instance.Lib?.EntityRevealed;
            
            PlayAudio();
            
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
