using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Map.TriggerTiles
{
    public class LevelCompletion : TriggerTileManager
    {
        private LevelManager _levelManager;
        private RelicPickup _relicPickup;
        private int _relicsRequiredForCompletion;

        private void Awake()
        {
            _levelManager = GetComponent<LevelManager>();
            _relicPickup = GetComponent<RelicPickup>();
        }
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (_relicsRequiredForCompletion > _relicPickup.RelicsGathered) return;
            if (_triggerLocationTiles == null) return;
            if (tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tileNode}");
            }
            _levelManager.LevelComplete();
        }
    }
}
