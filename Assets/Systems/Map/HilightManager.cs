using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam
{
    /**
    @Author: Luke Johnson
    Handles rendering the hilight tilemap with a hilight, when the function does this, 
    it renders the previous hilighted tilemap to null.
    also handles rendering the hilight tile invisible if you cannot move.
    **/
    public class HilightManager : MonoBehaviour
    {
        public TileBase CanMove;
        private Vector3Int? canMovePos;
        private Tilemap hilightMap;

        public void Awake()
        {
            hilightMap = GetComponent<Tilemap>();
        }

        /**
        first checks if the passed hilight tile equals the current hilight tile, if they do, does nothing,
then checks if the saved hilight coordinates are not null, if so s
removes the hilight from the rendered map by setting that tile to null.
then saves the passed hilight coordinates into the saved hilight coordinates
then reners the saved hilight coordinates to hilighted on the map.
        **/
        public void RenderCanMoveTile(Vector3Int canMovePos)
        {
            if (this.canMovePos == canMovePos) return;
            if (this.canMovePos != null)
            {
                hilightMap.SetTile(this.canMovePos.Value, null);
            }
            this.canMovePos = canMovePos;
            hilightMap.SetTile(canMovePos, CanMove);
        }
        /**
        checks if the saved coordinates are not null, if they are not, render the tilemap at that position null.
        **/
        public void HideCanMoveTile()
        {
            if (this.canMovePos != null)
            {
                hilightMap.SetTile(this.canMovePos.Value, null);
                this.canMovePos = null;
            }
        }
    }
}
