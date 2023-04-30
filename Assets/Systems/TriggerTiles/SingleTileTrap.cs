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
        private void OnValidate()
        {
            _triggerLocationTiles.Clear();
        }

        public override void SetupTriggerTiles()
        {
            Vector3Int coordinate = _ref.MapManager.Map.WorldToCell(this.transform.position);
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sprite = null;
            }
            
            // base.SetupTriggerTiles();
            if (_ref.TileNodeManager == null)
            {
                Debug.LogWarning($"SetupTriggerTiles did not have a reference to TileNodeManager.");
                return;
            }
            
            TileNode tileNode = _ref.TileNodeManager.GetNodeFromCoords(coordinate);
            if (tileNode == null)
            {
                Debug.LogWarning($"Attempting to set TriggerTile out of bounds: {coordinate}");
                return;
            }
            _tileNode = tileNode;
            _tileNode.SetUpTrigger(this, _triggerTile, _colour);
            
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
