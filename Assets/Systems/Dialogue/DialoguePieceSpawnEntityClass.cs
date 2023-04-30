using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;

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
        [SerializeField] protected Color _colour = Color.white;
        [SerializeField] protected MapManager _mapManager;
        protected MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnEntity();
            _ref.DialogueManager.WaitOnClick = true;
        }

        protected virtual void DoSpawnEntity()
        {
            if (_customAudioClip != null)
                SoundManager.Instance.PlaySound(_customAudioClip);
            else
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.EntityRevealed);

            // DialoguePieceSpawnEntityClass dialogueSpawnEntity = (DialoguePieceSpawnEntityClass)_currentDialogue[_dialogueIndex];
            _entitySpawned = _ref.EntityManager.SpawnEntity(_entityPrefab, _coords);
            if (EntitySpawned != null)
            Debug.Log($"Successfully spawned {_entitySpawned}");
            _ref.DialogueManager.WaitOnClick = true;
        }

        protected virtual void OnDrawGizmos()
        {
            if (Map == null) return;
            Gizmos.color = _colour;
            Vector3 position = Map.GetWorldPosFromGridCoord(_coords);
            Gizmos.DrawSphere(position, .2f);
        }    
    }
}
