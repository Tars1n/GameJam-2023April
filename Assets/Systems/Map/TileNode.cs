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
        [SerializeField] private bool _isSelectable;
        public bool IsSelectable => _isSelectable;
        [SerializeField] private bool _isWalkable;
        [SerializeField] private bool _isTrapTile;
        public Vector3Int GridCoordinate;
        public Vector3 WorldPos;
        public Vector3Int WalkingPathDirection;
        public Vector3Int FlyingPathDirection;
        public bool WalkingPathExplored = false;
        public bool FlyingPathExplored = false;
        public List<EntityBase> Entities = new List<EntityBase>();

        public void SetTileData(Dictionary<TileBase, TileData> data)
        {
            if (data.ContainsKey(TileType) == false)
                {return;}
            _isSelectable = data[TileType].IsSelectable;
            _isWalkable = data[TileType].IsWalkable;
            _isTrapTile = data[TileType].IsTrapTile;
        }

        public bool IsWalkable()
        {
            bool result = _isWalkable;
            if (Entities.Count > 0)
            {
                //? currently having an error where entity can't path out of their own tile when on
                //return false;
            }
            return result;
        }

        public EntityCharacter GetPlayerCharacter()
        {
            if (Entities.Count == 0)
            {
                return null;
            }
            foreach (EntityBase entity in Entities)
            {
                EntityCharacter character = IsValidCharacterSelection(entity);
                if (character != null)
                    return character;
            }
            return null;
        }

        private EntityCharacter IsValidCharacterSelection(EntityBase entity)
        {
            if (entity.GetType() != typeof(EntityCharacter) || entity.HasActionReady == false)
                { return null; }
                
            return (EntityCharacter)entity;
        }

        public void ResetPathingInfo()
        {
            WalkingPathDirection = GridCoordinate;
            FlyingPathDirection = GridCoordinate;
            FlyingPathExplored = false;
            WalkingPathExplored = false;
        }

        public void RecordPathing(Vector3Int prevTile, bool ignoreObstacles)
        {
            if (ignoreObstacles && !FlyingPathExplored)
            {
                FlyingPathDirection = prevTile;
                FlyingPathExplored = true;
            }
            if (!ignoreObstacles && !WalkingPathExplored)
            {
                WalkingPathDirection = prevTile;
                WalkingPathExplored = true;
            }
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
