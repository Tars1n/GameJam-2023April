using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Map;

namespace GameJam.Entity
{
    public class EntityBarrel : EntityObject
    {
        [SerializeField] private TileBase _barrelFilledHole;

        public override void LinkToTileNode(TileNode tileNode)
        {
            base.LinkToTileNode(tileNode);
            if (_currentTileNode == null) return;
            _spriteRenderer.color = _ref.MapManager.Map.GetColor(_currentTileNode.GridCoordinate);
        }


        public override void FallInPit()
        {
            Color tileColour = _ref.MapManager.Map.GetColor(_currentTileNode.GridCoordinate);
            _currentTileNode.SetTileType(_barrelFilledHole);
            _ref.MapManager.Map.SetTileFlags(_currentTileNode.GridCoordinate, TileFlags.None);
            _ref.MapManager.Map.SetColor(_currentTileNode.GridCoordinate, tileColour);
            _ref.EntityManager.DestroyEntity(this);
        }
    }
}
