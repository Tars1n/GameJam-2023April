using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Entity.Trap;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileNode
    {
        public TileBase TileType;
        [SerializeField] private bool _isSelectable;
        public bool IsSelectable => _isSelectable;
        [SerializeField] private bool _isWalkable;
        [SerializeField] private bool _isPitTile;
        public bool IsPitTile => _isPitTile;
        public Vector3Int GridCoordinate;
        public Vector3 WorldPos;
        public Vector3Int WalkingPathDirection;
        public Vector3Int FlyingPathDirection;
        public bool WalkingPathExplored = false;
        public bool FlyingPathExplored = false;
        private MapInteractionManager _mapInteractionManager;
        [SerializeField] private TriggerEventManager _triggerEventManager;
        public TriggerEventManager TriggerEventManager => _triggerEventManager;
        
        public List<EntityBase> Entities = new List<EntityBase>();

        private void Start()
        {
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
        }
        public void SetTileData(Dictionary<TileBase, TileData> data)
        {
            if (data.ContainsKey(TileType) == false)
                {return;}
            _isSelectable = data[TileType].IsSelectable;
            _isWalkable = data[TileType].IsWalkable;
            _isPitTile = data[TileType].IsPitTile;
        }

        public bool IsWalkable()
        {
            bool result = _isWalkable;
            if (GameMaster.Instance.ActiveEntity?.CurrentTileNode == this)
                { return true; }
            Entities.RemoveAll(entity => entity == null);
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
            Entities.Add(entity);
            _triggerEventManager?.EntityEnteredTrigger(entity, this);
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
        public void SetUpTrigger(TriggerEventManager triggerEventManager)
        {
            SetUpMapInteractionManager();
            _triggerEventManager = triggerEventManager;
            _mapInteractionManager?.RenderTriggerHilight(GridCoordinate);
        }
        public void ClearTrigger()
        {
            _mapInteractionManager?.ClearTriggerHilight(GridCoordinate);
            _triggerEventManager = null;
        }
        private void SetUpMapInteractionManager()
        {
            if (_mapInteractionManager != null) return;
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;        
        }

    }
}
