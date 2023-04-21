using System.Collections;
using System.Collections.Generic;
using GameJam.Entity;
using UnityEngine;

namespace GameJam.Map.TriggerTiles
{
    public class LeverTrigger : TriggerTileManager
    {
        [SerializeField] private bool _leverPulled = false;
        [SerializeField] private List<TileNode> _tilesToToggle = new List<TileNode>();
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnValidate()
        {
            if (_triggerLocationTiles.Count != 0)
            {
                _triggerLocationTiles.Clear();
            }
        }

        protected override void Start()
        {
            Vector3Int coordinate = Map.Map.WorldToCell(this.transform.position);
            _triggerLocationTiles.Add(coordinate);
            base.Start();
        }

        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            ToggleLever();
        }

        public void ToggleLever()
        {
            _leverPulled = !_leverPulled;
            _animator?.SetBool("LeverPulled", _leverPulled);
        }
    }
}
