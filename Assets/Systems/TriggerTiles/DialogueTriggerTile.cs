using System.Collections;
using System.Collections.Generic;
using GameJam.Entity;
using UnityEngine;
using GameJam.Dialogue;

namespace GameJam.Map.TriggerTiles
{
    public class DialogueTriggerTile : TriggerTileManager
    {
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase is EntityCharacter == false) return;

            if (_triggeredDialogue == null || _triggeredDialogue.Count == 0) return;

            _ref.DialogueManager.DoDialogue(_triggeredDialogue);
            
            ClearTriggerTiles();
            EntityBase eb = this.GetComponent<EntityBase>();
            if (eb != null)
                _ref.EntityManager.DestroyEntity(eb);
        }
    }
}
