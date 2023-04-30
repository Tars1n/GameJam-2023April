using System.Collections;
using System.Collections.Generic;
using GameJam.Entity;
using UnityEngine.Tilemaps;
using UnityEngine;

namespace GameJam.Map.TriggerTiles
{
    public class SingleTileTrap : TriggerTileManager
    {
        private TileNode _tileNode;
        private Vector3Int _coordinate;

        private void OnValidate()
        {
            _triggerLocationTiles.Clear();
            if (_mapManager == null) { return; }
            Vector3Int coordinate = Map.Map.WorldToCell(this.transform.position);
            _triggerLocationTiles.Add(coordinate);
        }

        public override void SetupTriggerTiles()
        {
            Debug.Log($"Begin SingleTile Setup.");
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();

            if (renderer != null)
                { renderer.sprite = null; }
            Vector3Int coordinate = Map.Map.WorldToCell(this.transform.position);
            _tileNode = _ref.TileNodeManager.GetNodeFromCoords(coordinate);

            if (_tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {_coordinate}");
                return;
            }

            Tile t = (Tile)_triggerTile;
            _tileNode.SetUpTrigger(this, _triggerTile, t.color);            
        }

        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.TrapActivated);
                entityBase.TriggerTrap();
                ClearTriggerTiles();
                // _ref.EntityManager.DestroyEntity(entityBase); //remove entity that entered trigger tile
                _ref.EntityManager.DestroyEntity(this?.GetComponent<EntityBase>()); //remove trap
            }
        }

        public override void ClearTriggerTiles()
        {
            if (_triggerLocationTiles == null || _ref.TileNodeManager == null) return;
            
            _tileNode.ClearTrigger();
        }
        
    }
}
