using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Entity.Abilities;

namespace GameJam.Map
{
    public class MirrorManager : MonoBehaviour
    {
        private MapManager _mapManager;
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        [SerializeField] private Vector3Int _mirrorOrigin;
        private Vector3Int _mirrorOriginAxial;
        [SerializeField] private bool _mirrorX = true;
        [SerializeField] private bool _mirrorY = false;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
            _mapInteractionManager = GetComponent<MapInteractionManager>();
            _tileNodeManager = GetComponent<TileNodeManager>();
            _mirrorOriginAxial = _mapManager.CastOddRowToAxial(_mirrorOrigin);
        }

        public TileNode TryReflectTileNode(TileNode tile)
        {
            if (!_mirrorX && !_mirrorY)
                { return null;}
            
            Vector3Int reflection = ReflectGridCoordinate(tile.GridCoordinate);
            return _tileNodeManager.GetNodeFromCoords(reflection);
        }

        public bool IsReflecting()
        {
            if (_mirrorX || _mirrorY)
                { return true; }
            return false;
        }

        public Vector3Int ReflectGridCoordinate(Vector3Int coord)
        {
            Vector3Int incomingAxial = _mapManager.CastOddRowToAxial(coord);
            Vector3Int outgoingAxial = incomingAxial - _mirrorOriginAxial; //make axis relative to mirror _mirrorOrigin
            if (_mirrorX)
                { outgoingAxial = ReflectAxialX(outgoingAxial); }
            if (_mirrorY)
                { ReflectAxialY(outgoingAxial); }

            outgoingAxial += _mirrorOriginAxial; //add relative mirror origin back into equation since it had been removed.

            return _mapManager.CastAxialToOddRow(outgoingAxial);
        }

        public Vector3Int ReflectAxialX(Vector3Int axialCoord)
        {
            return new Vector3Int(axialCoord.z, axialCoord.y, axialCoord.x); //swap axis ignoring y
        }

        public Vector3Int ReflectAxialY(Vector3Int axialCoord)
        {
            axialCoord = ReflectAxialX(axialCoord);
            axialCoord *= -1; //invert coordinate
            return axialCoord;
        }

        //if the system mirrors character action instead, coordinate isn't as critical, just mirroring the axial direction and distance amount.
        public void TryMirrorEntityAction(EntityBase originalEntity, TileNode selectedTile)
        {
            Mirrored mirrored = originalEntity.GetComponent<Mirrored>();
            if (mirrored == null) { return; }

            TileNode originTile = originalEntity.CurrentTileNode;
            Vector3Int originalPointingVector = _mapManager.CalculateAxialPointerBetweenTiles(originTile, selectedTile);

            if (mirrored.MirrorEntityX != null)
            {
                Vector3Int xPointer = ReflectAxialX(originalPointingVector);
                DoActionOnMirroredEntity(mirrored.MirrorEntityX, xPointer);
            }

            if (mirrored.MirrorEntityY != null)
            {
                Vector3Int yPointer = ReflectAxialY(originalPointingVector);
                DoActionOnMirroredEntity(mirrored.MirrorEntityY, yPointer);
            }
        }

        private void DoActionOnMirroredEntity(EntityBase mirroredEntity, Vector3Int mirroredPointer)
        {
            Vector3Int entityPos = _mapManager.CastOddRowToAxial(mirroredEntity.CurrentTileNode.GridCoordinate);
                mirroredPointer += entityPos;
                Vector3Int targetCoord = _mapManager.CastAxialToOddRow(mirroredPointer);
                TileNode tileNode = _tileNodeManager.GetNodeFromCoords(mirroredPointer);
                _mapInteractionManager.TryToTakeAction(mirroredEntity, tileNode);
        }
    }

}
