using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Map.TriggerTiles;
using GameJam.Dialogue;

namespace GameJam.Map
{
    [System.Serializable]
    public class TileNode
    {
        private ReferenceManager _ref => GameMaster.Instance.ReferenceManager;
        [SerializeField] TileBase _tileType;
        public TileBase TileType => _tileType;
        [SerializeField] private bool _isSelectable = false;
        public bool IsSelectable => _isSelectable;
        [SerializeField] private bool _isWalkable = false;
        [SerializeField] private bool _isPitTile = false;
        public bool IsPitTile => _isPitTile;
        [SerializeField] private bool _occlusionLayer;
        public bool OcclusionLayer => _occlusionLayer;
        public Vector3Int GridCoordinate;
        public Vector3 WorldPos;
        public Vector3Int WalkingPathDirection;
        public Vector3Int FlyingPathDirection;
        public bool WalkingPathExplored = false;
        public bool FlyingPathExplored = false;
        [SerializeField] private TriggerTileManager _triggerTileManager;
        public TriggerTileManager TriggerTileManager => _triggerTileManager;
        public List<EntityBase> Entities = new List<EntityBase>();        

        public void SetTileType(TileBase tileType)
        {
            _ref.MapManager.Map.SetTile(GridCoordinate, tileType);
            _tileType = tileType;
            SetTileData(_ref.TileNodeManager.DataFromTiles);
        }
        
        public void SetTileData(Dictionary<TileBase, TileAttributes> data)
        {
            ResetTileTypeData();
            if (data.ContainsKey(TileType) == false)
                {return;}
            _isSelectable = data[TileType].IsSelectable;
            _isWalkable = data[TileType].IsWalkable;
            _isPitTile = data[TileType].IsPitTile;
            _occlusionLayer = data[TileType].OcclusionLayer;
        }

        private void ResetTileTypeData()
        {
            _isSelectable = false;
            _isWalkable = false;
            _isPitTile = false;
            _occlusionLayer = false;
            if (GameMaster.Instance._jacobLogs) Debug.Log("TileData reset.");
        }


        public bool IsWalkable(EntityBase entity)
        {
            bool result = _isWalkable;
            Entities.TrimExcess();
            if (Entities.Contains(entity)) //walking into a tile it already occupies. 
                { return true; }

            if (Entities.Count > 0)
            {
                foreach (EntityBase tileEntity in Entities)
                {
                    if (tileEntity.BlocksMovement)
                        return false;
                }
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
            {
                Entities.Add(entity);
                if (GameMaster.Instance._jacobLogs)
                    Debug.Log($"Tilenode successfully added {entity}");
            }
            if (_isPitTile)
            {
                EntityEnteredPit(entity);
                return true;
            }
            _triggerTileManager?.EntityEnteredTrigger(entity, this);
            if (_ref.LevelManager.RecordSlimeTrails)
            {
                GameObject slime = GameObject.Instantiate(_ref.LevelManager.SlimeDrop, WorldPos, Quaternion.identity);
            }
            return true;
        }

        //if entity is beings shoved over pit, first stops their movement before they can be destroyed by it.
        private void EntityEnteredPit(EntityBase entity)
        {
            if (GameMaster.Instance._jacobLogs)
                    Debug.Log($"{entity} entered pit!");
            entity.FallInPit();
        }

        public bool TryRemoveEntity(EntityBase entity)
        {
            if (Entities.Contains(entity))
            {
                Entities.Remove(entity);
                Entities.TrimExcess();
                if (GameMaster.Instance._jacobLogs)
                    Debug.Log($"{this} successfully removed {entity}");
                return true;
            }
            Entities.TrimExcess();
            if (GameMaster.Instance._jacobLogs)
                Debug.LogWarning($"{this} failed to removed {entity}");
            return false;
        }

        public void ClearEntityList()
        {
            Entities = new List<EntityBase>();
        }
        public void SetUpTrigger(TriggerTileManager triggerTileManager, TileBase triggerTile, Color colour)
        {
            _triggerTileManager = triggerTileManager;
            _ref.MapInteractionManager.RenderTriggerHilight(GridCoordinate, triggerTile, colour);
        }
        public void ClearTrigger()
        {
            _ref.MapInteractionManager?.ClearTriggerHilight(GridCoordinate);
            _triggerTileManager = null;
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
                Debug.Log($"{entity} bumped.");
                entity.CollidedWithObject();
            }
        }

    }
}
