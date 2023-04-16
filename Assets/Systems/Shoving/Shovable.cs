using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity.Shoving
{
    public class Shovable : MonoBehaviour
    {
        private MapManager _mapManager;
        private EntityBase _entityBase;
        private TileNodeManager _tileNodeManager;
        private MapInteractionManager _mapInteractionManager;

        private void Awake()
        {
            _entityBase = GetComponent<EntityBase>();
        }
        private void Start()
        {
            _mapManager = GameMaster.Instance.ReferenceManager.MapManager;
            _tileNodeManager = GameMaster.Instance.ReferenceManager.TileNodeManager;
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
        }
        public void TryShoveDir(Vector3Int axialDir, int distance)
        {
            Vector3Int currentAxialCoords = _mapManager.CastOddRowToAxial(_entityBase.CurrentTileNode.GridCoordinate);
            TileNode tileMovingTo = null;
            TileNode projectedTile = null;
            TileNode collisionTile = null;
            float journeyCompleted = 0f;
            float stepsTaken = 0f;
            for (int i = distance; i > 0; i--)
            {   //keep projecting the shove by one axial unit until distance reached or hit obstacle
                journeyCompleted = stepsTaken/distance;
                stepsTaken++;
                currentAxialCoords += axialDir;
                Vector3Int coords = _mapManager.CastAxialToOddRow(currentAxialCoords);
                projectedTile = _tileNodeManager.GetTileFromAxial(currentAxialCoords);
                if (projectedTile == null)
                {
                    Debug.LogError($"invalid tilenode being shoved into. {currentAxialCoords} with current range value of {i}");
                    projectedTile = tileMovingTo;
                    collisionTile = tileMovingTo;
                    break;
                }
                if (projectedTile.IsWalkable())
                {
                    journeyCompleted = stepsTaken/distance;
                    tileMovingTo = projectedTile;
                    Debug.LogWarning($"valid walkable tile: {tileMovingTo.GridCoordinate}");
                }
                else
                { 
                        // collisionTile = projectedTile;
                }
            }
            if (tileMovingTo != null)
            {   //valid tile to be shoved to, move entity
                _mapInteractionManager.ShoveEntity(_entityBase, tileMovingTo, journeyCompleted, collisionTile);
            }
            if (tileMovingTo != projectedTile)
            {   //if tileMovingTo is different from Projected tile, that means an obstacle was in the way, cause all entities at projected tile to be hit
                projectedTile.CollidedWith();
            }
            //can't move anywhere so hops in place and loses turn.
            //_mapInteractionManager.MoveEntityUpdateTileNodes(_entityBase, _entityBase.CurrentTileNode);
        }
    }
}
