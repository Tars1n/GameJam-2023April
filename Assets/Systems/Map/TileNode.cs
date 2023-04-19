using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Map.TriggerTiles;
using UnityEngine.Tilemaps;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileNode
    {
        public TileBase TileType;
        [SerializeField] private bool _isSelectable = false;
        public bool IsSelectable => _isSelectable;
        [SerializeField] private bool _isWalkable = false;
        [SerializeField] private bool _isPitTile = false;
        public bool IsPitTile => _isPitTile;
        public Vector3Int GridCoordinate;
        public Vector3 WorldPos;
        public Vector3Int WalkingPathDirection;
        public Vector3Int FlyingPathDirection;
        public bool WalkingPathExplored = false;
        public bool FlyingPathExplored = false;
        private MapInteractionManager _mapInteractionManager;
        private LevelManager _levelManager;
        public LevelManager LevelManager => _levelManager ? _levelManager : _levelManager = GameMaster.Instance.ReferenceManager.LevelManager;
        [SerializeField] private TriggerTileManager _triggerTileManager;
        public TriggerTileManager TriggerTileManager => _triggerTileManager;
        
        public List<EntityBase> Entities = new List<EntityBase>();

        private void Start()
        {
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
            _levelManager = GameMaster.Instance.ReferenceManager.LevelManager;
        }
        public void SetTileData(Dictionary<TileBase, TileData> data)
        {
            if (data.ContainsKey(TileType) == false)
                {return;}
            _isSelectable = data[TileType].IsSelectable;
            _isWalkable = data[TileType].IsWalkable;
            _isPitTile = data[TileType].IsPitTile;
        }

        public bool IsWalkable(EntityBase entity)
        {
            bool result = _isWalkable;
            Entities.TrimExcess();
            if (Entities.Contains(entity)) //used to contain: GameMaster.Instance.ActiveEntity?.CurrentTileNode == this || 
                { return true; }
            if (Entities.Count > 0)
            {
                return false;
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
            if (Entities.Contains(entity) == false)
                {Entities.Add(entity);}
            if (_isPitTile)
            {
                GameMaster.Instance.ReferenceManager.EntityManager.TryRemoveEntity(entity);
                return false;
            }
            _triggerTileManager?.EntityEnteredTrigger(entity, this);
            if (LevelManager.RecordSlimeTrails)
            {
                GameObject slime = GameObject.Instantiate(LevelManager.SlimeDrop, WorldPos, Quaternion.identity);
            }
            return true;
        }

        public bool TryRemoveEntity(EntityBase entity)
        {
            if (Entities.Contains(entity))
            {
                Entities.Remove(entity);
                Entities.TrimExcess();
                return true;
            }
            Entities.TrimExcess();
            return false;
        }

        public void ClearEntityList()
        {
            Entities = new List<EntityBase>();
        }
        public void SetUpTrigger(TriggerTileManager triggerTileManager, TileBase triggerTile)
        {
            SetUpMapInteractionManager();
            _triggerTileManager = triggerTileManager;
            _mapInteractionManager?.RenderTriggerHilight(GridCoordinate, triggerTile);
        }
        public void ClearTrigger()
        {
            _mapInteractionManager?.ClearTriggerHilight(GridCoordinate);
            _triggerTileManager = null;
        }
        private void SetUpMapInteractionManager()
        {
            if (_mapInteractionManager != null) return;
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;        
        }

        public void CollidedWith()
        {
            List<EntityBase> entitiesToBump = new List<EntityBase>();
            foreach (EntityBase entity in Entities)
            {
                entitiesToBump.Add(entity);
            }
            //had to cast to a new list as they would get removed from TileNode.Entities while enumeration was happening.
            foreach (EntityBase entity in entitiesToBump)
            {
                entity.CollidedWithObject();
            }
        }

    }
}
