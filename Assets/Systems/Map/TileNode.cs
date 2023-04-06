using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileNode
    {
        public TileBase TileType;
        public Vector3Int GridPosition;
        public Vector3 WorldPos;
        public Vector3Int PreviousStepGridPosition;
        public bool PathExplored = false;
        public List<EntityBase> Entities = new List<EntityBase>();

        public void ResetPathingInfo()
        {
            PreviousStepGridPosition = GridPosition;
            PathExplored = false;
        }

        public void RecordPathing(Vector3Int prevTile)
        {
            PreviousStepGridPosition = prevTile;
            PathExplored = true;
        }

        public bool TryAddEntity(EntityBase entity)
        {
            Entities.Add(entity);
            return true;
        }

        public bool TryRemoveEntity(EntityBase entity)
        {
            if (Entities.Contains(entity))
            {
                Entities.Remove(entity);
                return true;
            }
            return false;
        }

        public void ClearEntities()
        {
            Entities.Clear();
        }
    }
}
