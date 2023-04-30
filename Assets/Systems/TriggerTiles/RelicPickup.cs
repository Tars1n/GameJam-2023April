using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Level;
using GameJam.UI;

namespace GameJam.Map.TriggerTiles
{
    public class RelicPickup : TriggerTileManager
    {
        [SerializeField] private int _relicsGathered;
        public int RelicsGathered => _relicsGathered;
        private ScoreDisplay _scoreDisplay;

        private LevelManager _levelManager;

        private void Awake()
        {
            _levelManager = GetComponent<LevelManager>();
        }
        private void Start()
        {
            _scoreDisplay = GameMaster.Instance.ReferenceManager.ScoreDisplay;
        }
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase.GetType() != typeof(EntityCharacter))
            {
                //entity stepping on relic is not player
                return;
            }

            if (_triggerLocationTiles == null || _triggerLocationTiles.Count == 0) return;
            if (tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {tileNode}");
            }
            tileNode.ClearTrigger();
            _relicsGathered ++;
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.GatheredRelic);
            _levelManager.ScoreSO.AddRelics(1);
            _scoreDisplay?.DisplayScore();
        }
    }
}
