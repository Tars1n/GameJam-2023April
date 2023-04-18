using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;

namespace GameJam.TriggerEvents
{
    public class Oneshot_DeathTiles : TriggerEventManager
    {
        public override void EntityEnteredTrigger(EntityBase entityBase, TileNode tileNode)
        {
            if (entityBase != null)
            {
                ClearTriggerTiles();
                _entityManager.TryRemoveEntity(entityBase); //remove entity that entered trigger tile
                _entityManager.TryRemoveEntity(this?.GetComponent<EntityBase>()); //remove trap
            }
        }
    }
}
