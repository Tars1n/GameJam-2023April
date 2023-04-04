using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Map;

namespace Assets.Scripts.Unity.Tilempas
{
    public static class TileMapExtensionMethods
    {
        public static void SetAllTilesArray(this Tilemap _tilemap, TileClassArrayManager _tileClassArrayManager)
        {
            var bounds = _tilemap.cellBounds;
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    TileBase tile = _tilemap.GetTile(cellPosition);
                    if (tile == null)
                    {
                        continue;
                    }
                    _tileClassArrayManager.CheckIfClassAtCoord(cellPosition);

                }
            }
        }
    }
}
