using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceSpawnEntityClass : DialoguePieceClass
    {
        [SerializeField] protected GameObject _entityPrefab;
        public GameObject EntityPrefab => _entityPrefab;
        [SerializeField] protected Vector3Int _coords;
        protected EntityBase _entitySpawned;
        public EntityBase EntitySpawned => _entitySpawned;
        

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnEntity();
            _ref.DialogueManager.WaitOnClick = true;
        }

        protected virtual void DoSpawnEntity()
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);
            // DialoguePieceSpawnEntityClass dialogueSpawnEntity = (DialoguePieceSpawnEntityClass)_currentDialogue[_dialogueIndex];
            _entitySpawned = _ref.EntityManager.SpawnEntity(_entityPrefab, _coords);
            if (EntitySpawned != null)
            Debug.Log($"Successfully spawned {_entitySpawned}");
            _ref.DialogueManager.WaitOnClick = true;
        }        
    }
}
