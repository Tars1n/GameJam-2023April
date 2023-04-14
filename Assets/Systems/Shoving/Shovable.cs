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
        public void TryShoveDir(Vector3Int axialDir)
        {
            Vector3Int currentAxialCoords = _mapManager.CastOddRowToAxial(_entityBase.CurrentTileNode.GridCoordinate);
            Vector3Int newAxialCoords = currentAxialCoords + axialDir;
            TileNode tileMovingTo = _tileNodeManager.GetNodeFromCoords(_mapManager.CastAxialToOddRow(newAxialCoords));
            if (tileMovingTo.IsWalkable())
            {
                _mapInteractionManager.ShoveEntity(_entityBase, tileMovingTo);
            }
            else
            {
                //can't move anywhere so hops in place and loses turn.
                _mapInteractionManager.MoveEntityUpdateTileNodes(_entityBase, _entityBase.CurrentTileNode);
            }

        }
    }
}
