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
        public List<EntityBase> Entities;

        public void ResetPathingInfo()
        {
            PreviousStepGridPosition = GridPosition;
        }

        public void AddEntity(EntityBase entity)
        {
            Entities.Add(entity);
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
