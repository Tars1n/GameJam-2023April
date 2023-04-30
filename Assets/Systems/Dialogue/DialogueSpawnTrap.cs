using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map.TriggerTiles;

namespace GameJam.Dialogue
{
    public class DialogueSpawnTrap : DialoguePieceSpawnEntityClass
    {
        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoSpawnEntity();
            _entitySpawned.GetComponent<TriggerTileManager>()?.SetupTriggerTiles();
        }

        
    }
}
