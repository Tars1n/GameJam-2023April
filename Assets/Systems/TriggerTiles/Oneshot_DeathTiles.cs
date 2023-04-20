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
                ClearTriggerTiles();
                _entityManager.TryDestroyEntity(entityBase); //remove entity that entered trigger tile
                _entityManager.TryDestroyEntity(this?.GetComponent<EntityBase>()); //remove trap
            }
        }
    }
}
