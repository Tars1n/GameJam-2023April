using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity
{
    public class KillAdjacentPlayer : MonoBehaviour
    {
        private TileNodeManager _tileNodeManager;
        private MapManager _mapManager;
        private EntityBase _entityBase;
        private EntityManager _entityManager;

        private void Start()
        {
            _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
            _mapManager =GameMaster.Instance.ReferenceManager.MapManager;
            _entityBase = GetComponent<EntityBase>();
            _entityManager = GameMaster.Instance.ReferenceManager.EntityManager;

        }
        public void KillIfCan()
        {
            if (_entityBase.HasActionReady == false)
                { return; }
            Vector3Int[] adjacentTiles = _mapManager.GetAllAdjacentHexCoordinates(_entityBase.CurrentTileNode.GridCoordinate);
            EntityCharacter targetToKill = null;
            foreach (Vector3Int vector3Int in adjacentTiles)
            {
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(vector3Int + _entityBase.CurrentTileNode.GridCoordinate);
                if ((tileNode != null) && (tileNode.Entities != null) && (tileNode.Entities.Count > 0))
                {
                    foreach (EntityBase entity in tileNode?.Entities)
                    {
                        if (entity?.GetType() == typeof(EntityCharacter))
                        {
                            targetToKill = (EntityCharacter)entity;
                        }
                    }
                }
            }
            if (targetToKill != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.CultistAttackedPlayer);
                _entityManager.DestroyEntity(targetToKill);
            }
        }
    }
}
