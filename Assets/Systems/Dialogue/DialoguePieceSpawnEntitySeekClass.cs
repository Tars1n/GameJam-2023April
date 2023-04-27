using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceSpawnEntitySeekClass : DialoguePieceSpawnEntityClass
    {
        [SerializeField] private EntityBase _entityToSeek;
        public EntityBase EntityToSeek => _entityToSeek;
    }
}
