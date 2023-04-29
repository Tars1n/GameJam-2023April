using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceSpawnEntityClass : DialoguePieceClass
    {
        [SerializeField] private GameObject _entityPrefab;
        public GameObject EntityPrefab => _entityPrefab;
        [SerializeField] private Vector3 _entitySpawnWorldPos;
        public Vector3 EntitySpawnWorldPos => _entitySpawnWorldPos;

        public override void DoPiece(DialogueManager dialogueManager)
        {
            
        }
    }
}
