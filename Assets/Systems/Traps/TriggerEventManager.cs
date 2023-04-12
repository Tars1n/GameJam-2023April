using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity.Trap
{
    public class TriggerEventManager : MonoBehaviour
    {
        private TileNodeManager _tileNodeManager;
        private ReferenceManager _ref;
        [SerializeField] private List<Vector3Int> _triggerLocationTiles;

        private void Start()
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _tileNodeManager = _ref.TileNodeManager;
            SetUpTrap();
        }
        public void SetUpTrap()
        {
            if (_triggerLocationTiles == null) return;
            foreach (Vector3Int tile in _triggerLocationTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(tile);
                tileNode.SetUpTrigger(this);
            }
        }
    }
}
