using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;

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
                if (entity.IsShovable)
                    { return true; }
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
                if (entity == null) { continue; }
                if (entity.IsShovable)
                    { StartCoroutine(DoShoveEntity(entity, shoveDir, distance)); }
            }
        }

        IEnumerator DoShoveEntity(EntityBase entity, Vector3Int axialDir, int distance)
        {
            GameMaster.Instance.TilemapInteractable = false;
            GameMaster.Instance.AddEntityInMotion(entity);
            entity.IsCurrentlyMoving = true;

            Vector3 startPos = entity.transform.position;
            //calculate the final target position.
            Vector3Int currentCoord = entity.CurrentTileNode.GridCoordinate;
            Vector3Int axialTarget = _mapManager.CastOddRowToAxial(currentCoord);
            Vector3Int finalCoord = _mapManager.CastAxialToOddRow(axialTarget + (axialDir * distance));
            Vector3 targetWorldPos = _mapManager.GetWorldPosFromGridCoord(finalCoord);

            TileNode currentTile = entity.CurrentTileNode;
            TileNode projectedTile = currentTile;
            bool collisionHappened = false;

            
            float timeElapsed = 0;
            float j = 0f;
            float shortenSlideDistance = 0.1f;
            bool shoveOneTile = false;
            
            SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.StartShove);

            while (timeElapsed < _slideSpeed)
            {
                if (entity == null)
                {
                    Debug.LogWarning("DoShove was running on Null entity.");
                    GameMaster.Instance.TilemapInteractable = true;
                    GameMaster.Instance.RemoveEntityInMotion(entity);
                    yield break;
                }
                // float t = timeElapsed / duration;
                // t = t * t * (3f - 2f *t);
                float g = timeElapsed/_slideSpeed;
                g = 1 - ((1 - g)*(1 - g));

                float x = Mathf.Lerp(startPos.x, targetWorldPos.x, g);
                float y = Mathf.Lerp(startPos.y, targetWorldPos.y, g);

                if (entity.IsCurrentlyMoving)
                    entity.transform.position = new Vector3(x, y, 0);

                timeElapsed += Time.deltaTime;

                float journey = g*distance;
                shortenSlideDistance = .5f;
                
                if (journey >= j && collisionHappened == false && entity.IsCurrentlyMoving)
                {
                    j++;
                    shoveOneTile = true;
                    //get projected tile.
                    axialTarget += axialDir;
                    projectedTile = _tileNodeManager.GetTileFromAxial(axialTarget);
                    //Determine slide distance based on if entity can slide into upcoming tile.
                    if (projectedTile == null) {continue;}
                    if (projectedTile.IsWalkable(entity))
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.Sliding);
                        shortenSlideDistance = 0.1f;
                    }
                }
                if (shoveOneTile == true && journey >= (j - shortenSlideDistance) && collisionHappened == false)
                {
                    shoveOneTile = false;
                    TryShoveIntoTile(projectedTile);
                }
                if (collisionHappened)
                {
                    entity.IsCurrentlyMoving = false;
                    GameMaster.Instance.TilemapInteractable = true;
                    GameMaster.Instance.RemoveEntityInMotion(entity);
                    yield break;
                }

                yield return null;
            }

            // final attempt after while loop, because lerp never actually reaches 100%
            if (collisionHappened == false && entity.IsCurrentlyMoving)
            {
                projectedTile = _tileNodeManager.GetNodeFromCoords(finalCoord);
                if (entity.CurrentTileNode != null && entity.CurrentTileNode != projectedTile)
                    { TryShoveIntoTile(projectedTile); }
            }
            entity.IsCurrentlyMoving = false;

            bool TryShoveIntoTile(TileNode tileToCheck)
            {
                if (tileToCheck == null)
                {
                    entity.LinkToTileNode(currentTile);
                    collisionHappened = true;
                    currentTile.CollidedWith();
                    SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.Collision);
                    return false;
                }
                if (tileToCheck.IsWalkable(entity))
                {
                    currentTile = tileToCheck;
                    if (_shoveInteractWithEveryTile)
                        { entity.LinkToTileNode(currentTile); }
                    return true;
                }
                
                if (_debugLogs) Debug.Log("entity slammed into object");
                entity.LinkToTileNode(currentTile);
                collisionHappened = true;
                currentTile.CollidedWith();
                tileToCheck.CollidedWith();
                SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.Collision);
                return false;
            }

            GameMaster.Instance.TilemapInteractable = true;
            GameMaster.Instance.RemoveEntityInMotion(entity);
        }

    }
}
