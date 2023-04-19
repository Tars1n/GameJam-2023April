using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Map.TriggerTiles
{
    public class LevelCompletion : TriggerTileManager
    {
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (_triggerLocationTiles == null) return;
            if (tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tileNode}");
            }
            tileNode.ClearTrigger();
        }
    }
}
