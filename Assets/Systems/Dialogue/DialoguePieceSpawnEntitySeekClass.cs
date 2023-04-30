using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Entity.Brain;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceSpawnEntitySeekClass : DialoguePieceSpawnEntityClass
    {
        [SerializeField] private EntityBase _entityToSeek;
        public EntityBase EntityToSeek => _entityToSeek;

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnEntity();
            SetSeeking();
            FinishPiece();
        }
        private void SetSeeking()
        {
            BrainSeekTarget brainSeek = _entitySpawned?.GetComponent<BrainSeekTarget>();
            if (brainSeek == null || _entityToSeek == null)
            {   
                Debug.LogWarning("Dialogue attempting to spawn entity is not configured correctly. Missing target or was unable to spawn.");
                return;
            }
            brainSeek.SetTargetEntity(_entityToSeek);
        }
    }
    
}
