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
        public Vector3Int GridCoordinate;
        public Vector3 WorldPos;
        public Vector3Int PreviousStepGridCoordinate;
        public bool PathExplored = false;
        public List<EntityBase> Entities = new List<EntityBase>();

        public void ResetPathingInfo()
        {
            PreviousStepGridCoordinate = GridCoordinate;
            PathExplored = false;
        }

        public void RecordPathing(Vector3Int prevTile)
        {
            PreviousStepGridCoordinate = prevTile;
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
