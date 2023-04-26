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
        [SerializeField] private Vector3Int _entityLocation;
        public Vector3Int EntityLocation => _entityLocation;
    }
}
