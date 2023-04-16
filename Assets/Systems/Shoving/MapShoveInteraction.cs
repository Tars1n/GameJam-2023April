using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Entity.Shoving;

namespace GameJam.Map
{
    public class MapShoveInteraction : MonoBehaviour
    {
        [SerializeField] private bool _debugLogs = false;
        private MapManager _mapManager;
        private TileNodeManager _tileNodeManager;
        // private MapInteractionManager _mapInteractionManager;
        private Tilemap _mouseMap;
        [SerializeField] private bool _shoveInteractWithEveryTile = true;
        [SerializeField] private TileBase[] _shoveTileHilight;
        [SerializeField] private float _slideSpeed = 0.5f;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            // _mapInteractionManager = GetComponent<MapInteractionManager>();
            _mouseMap = _mapManager.MouseInteractionTilemap;
        }

        public void TryRenderShoveHilight(TileNode sourceTile, TileNode tileBeingPushed)
        {
            if (EntityOnThisTileThatCanBeShoved(tileBeingPushed))
            {
                _mouseMap.SetTile(tileBeingPushed.GridCoordinate, GetPushHilight(sourceTile.GridCoordinate, tileBeingPushed.GridCoordinate));
            }
        }
        public bool EntityOnThisTileThatCanBeShoved(TileNode tileBeingPushed)
        {
            if (tileBeingPushed == null) { return false; }
            if ((tileBeingPushed.Entities == null) || (tileBeingPushed.Entities.Count == 0)) return false;
            foreach (EntityBase entity in tileBeingPushed.Entities)
            {
                if (entity == null) { continue; }
                if (entity?.GetComponent<Shovable>() != null)
                {
                    return true;
                }
            }
            return false;
        }
        private TileBase GetPushHilight(Vector3Int sourceCoords, Vector3Int targetCoords)
        {
            Vector3Int axialDifference = GetAxialDifference(sourceCoords, targetCoords);
            int indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 1)) indexOfTileBase = 0;
            if ((axialDifference.x == -1) && (axialDifference.y == 0)) indexOfTileBase = 1;
            if ((axialDifference.x == 0) && (axialDifference.y == -1)) indexOfTileBase = 2;
            if ((axialDifference.x == 1) && (axialDifference.y == -1)) indexOfTileBase = 3;
            if ((axialDifference.x == 1) && (axialDifference.y == 0)) indexOfTileBase = 4;
            if ((axialDifference.x == 0) && (axialDifference.y == 1)) indexOfTileBase = 5;
            if (_debugLogs) Debug.Log($"shoving in direction " + indexOfTileBase);
            return (_shoveTileHilight[indexOfTileBase]);
        }
        private Vector3Int GetAxialDifference(Vector3Int sourceCoords, Vector3Int targetCoords)
        {
            Vector3Int axialSourceCoords = _mapManager.CastOddRowToAxial(sourceCoords);
            Vector3Int axialTargetCoords = _mapManager.CastOddRowToAxial(targetCoords);
            return axialTargetCoords - axialSourceCoords;
        }
        public void ShoveThisTile(TileNode sourceOfShove, TileNode targetOfShove, int distance)
        {
            if (_debugLogs) Debug.Log($"shoving from {sourceOfShove.GridCoordinate} to " + targetOfShove.GridCoordinate);
            if ((targetOfShove.Entities == null) || (targetOfShove.Entities.Count == 0)) return;
            Vector3Int shoveDir = GetAxialDifference(sourceOfShove.GridCoordinate, targetOfShove.GridCoordinate);
            List<EntityBase> copiedEntityList = new List<EntityBase>();
            foreach (EntityBase entity in targetOfShove.Entities)
            {
                copiedEntityList.Add(entity);
            }
            foreach (EntityBase entity in copiedEntityList)
            {
                Shovable shovable = entity.GetComponent<Shovable>();
                if (shovable == null) continue;
                TryShoveDir(entity, shoveDir, distance);
            }
        }

        public void TryShoveDir(EntityBase entity, Vector3Int axialDir, int distance)
        {
            Vector3Int currentAxialCoords = _mapManager.CastOddRowToAxial(entity.CurrentTileNode.GridCoordinate);
            Vector3Int projectedCoord = _mapManager.CastAxialToOddRow(currentAxialCoords);
            TileNode tileMovingTo = entity.CurrentTileNode;
            TileNode projectedTile = null;
            TileNode collisionTile = null;
            float journeyCompleted = 0f;
            float stepsTaken = 0f;
            for (int i = distance; i > 0; i--)
            {   //keep projecting the shove by one axial unit until distance reached or hit obstacle
                journeyCompleted = stepsTaken/distance;
                stepsTaken++;
                currentAxialCoords += axialDir;
                projectedCoord = _mapManager.CastAxialToOddRow(currentAxialCoords);
                projectedTile = _tileNodeManager.GetNodeFromCoords(projectedCoord);
                if (projectedTile == null)
                {
                    Debug.LogError($"invalid tilenode being shoved into. {currentAxialCoords} with current range value of {i}");
                    projectedTile = tileMovingTo;
                    collisionTile = tileMovingTo;
                    continue;
                }
                if (projectedTile.IsWalkable())
                {
                    journeyCompleted = stepsTaken/distance;
                    tileMovingTo = projectedTile;
                    // Debug.LogWarning($"valid walkable tile: {tileMovingTo.GridCoordinate}");
                }
                else
                { 
                        collisionTile = projectedTile;
                }
            }
            if (tileMovingTo != null)
            {   //valid tile to be shoved to, move entity
                ShoveEntity(entity, projectedCoord, journeyCompleted, collisionTile);
            }
            // if (tileMovingTo != projectedTile)
            // {   //if tileMovingTo is different from Projected tile, that means an obstacle was in the way, cause all entities at projected tile to be hit
            //     projectedTile.CollidedWith();
            // }
            //can't move anywhere so hops in place and loses turn.
            //_mapInteractionManager.MoveEntityUpdateTileNodes(_entityBase, _entityBase.CurrentTileNode);
        }

        public void ShoveEntity(EntityBase entity, Vector3Int targetCoord, float journeyCompleted, TileNode collisionTile)
        {
            if (entity?.CurrentTileNode == null)
                { return; }
            GameObject entityGO = entity.gameObject;
            Vector3 position = _mapManager.GetWorldPosFromGridCoord(targetCoord);
            
            StartCoroutine(DoShoveEntityToPos(entityGO, position, _slideSpeed, journeyCompleted, collisionTile));
        }

        IEnumerator DoShoveEntityToPos(GameObject entityGO, Vector3 targetPosition, float duration, float journeyCompleted, TileNode collisionTile)
        {
            Debug.LogWarning($"journey Completed: {journeyCompleted}");
            float timeElapsed = 0;
            EntityBase entity = entityGO.GetComponent<EntityBase>();
            Vector3 startPos = entityGO.transform.position;
            while (timeElapsed < duration)
            {
                // float t = timeElapsed / duration;
                // t = t * t * (3f - 2f *t);
                float g = timeElapsed/duration;
                g = 1 - ((1 - g)*(1 - g));

                float x = Mathf.Lerp(startPos.x, targetPosition.x, g);
                float y = Mathf.Lerp(startPos.y, targetPosition.y, g);

                entityGO.transform.position = new Vector3(x, y, 0);
                timeElapsed += Time.deltaTime;
                
                if (g >= journeyCompleted)
                {
                    if (collisionTile != null)
                    {
                        collisionTile.CollidedWith();
                    }
                    entity.LinkToTileNode(null);
                    entity.CurrentTileNode.CollidedWith();                    
                    entity.SnapEntityPositionToTile();
                    yield break;
                }

                yield return null;
            }
            entityGO.transform.position = targetPosition; 
            entity.LinkToTileNode(null);
            // entity.SnapEntityPositionToTile();
        }

        IEnumerator DoShoveEntity(EntityBase entity, Vector3Int axialDir, int distance)
        {
            GameMaster.Instance.TilemapInteractable = false;

            Vector3 startPos = entity.transform.position;
            //calculate the final target position.
            Vector3Int currentCoord = entity.CurrentTileNode.GridCoordinate;
            Vector3Int axialTarget = _mapManager.CastOddRowToAxial(currentCoord);
            Vector3Int finalCoord = _mapManager.CastAxialToOddRow(axialTarget + (axialDir * distance));
            Vector3 targetWorldPos = _mapManager.GetWorldPosFromGridCoord(finalCoord);

            TileNode currentTile = entity.CurrentTileNode;
            TileNode projectedTile = currentTile;
            bool collisionHappened = false;

            entity.LeaveTileNode();
            
            float timeElapsed = 0;
            int j = 0;

            while (timeElapsed < _slideSpeed)
            {
                // float t = timeElapsed / duration;
                // t = t * t * (3f - 2f *t);
                float g = timeElapsed/_slideSpeed;
                g = 1 - ((1 - g)*(1 - g));

                float x = Mathf.Lerp(startPos.x, targetWorldPos.x, g);
                float y = Mathf.Lerp(startPos.y, targetWorldPos.y, g);

                entity.transform.position = new Vector3(x, y, 0);
                timeElapsed += Time.deltaTime;

                float journey = g*distance;
                
                if (journey >= j && collisionHappened == false)
                {
                    j++;
                    //get projected tile.
                    axialTarget += axialDir;
                    projectedTile = _tileNodeManager.GetTileFromAxial(axialTarget);
                    TryShoveIntoTile(projectedTile);
                    if (collisionHappened)
                        { yield break; }
                }

                yield return null;
            }

            // final attempt after while, because lerp never actually reaches 100%
            if (collisionHappened == false)
            {
                projectedTile = _tileNodeManager.GetNodeFromCoords(finalCoord);
                TryShoveIntoTile(projectedTile);
            }

            bool TryShoveIntoTile(TileNode tileToCheck)
            {
                if (tileToCheck == null)
                {
                    entity.LinkToTileNode(currentTile);
                    currentTile.CollidedWith();
                    collisionHappened = true;
                    return false;
                }
                if (tileToCheck.IsWalkable())
                {
                    currentTile = tileToCheck;
                    if (_shoveInteractWithEveryTile)
                        { entity.LinkToTileNode(currentTile); }
                    return true;
                }
                
                entity.LinkToTileNode(currentTile);
                currentTile.CollidedWith();
                tileToCheck.CollidedWith();
                collisionHappened = true;
                return false;
            }      
            GameMaster.Instance.TilemapInteractable = true;
        }

    }
}
