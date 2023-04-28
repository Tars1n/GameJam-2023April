using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;

namespace GameJam.Map.TriggerTiles
{
    public class Oneshot_DeathTiles : TriggerTileManager
    {
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase != null)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.TrapActivated);
                ClearTriggerTiles();
                // _ref.EntityManager.DestroyEntity(entityBase); //remove entity that entered trigger tile
                _ref.EntityManager.DestroyEntity(this?.GetComponent<EntityBase>()); //remove trap
                entityBase.TriggerTrap();
            }
        }
    }
}
