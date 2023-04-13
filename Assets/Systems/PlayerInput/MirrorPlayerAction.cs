using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;

namespace GameJam.PlayerInput
{
    public class MirrorPlayerAction : MonoBehaviour
    {
        private MapManager _mapManager;
        private TileNodeManager _tileNodeManager;
        [SerializeField] private Vector3Int _mirrorOrigin;
        private Vector3Int _mirrorOriginAxial;
        [SerializeField] private bool _mirrorX = true;
        [SerializeField] private bool _mirrorY = false;

        private void Awake()
        {
            _mapManager = GetComponent<MapManager>();
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
            {
                outgoingAxial = new Vector3Int(outgoingAxial.z, outgoingAxial.y, outgoingAxial.x); //swap axis ignoring y
            }
            if (_mirrorY)
            {
                outgoingAxial = new Vector3Int(outgoingAxial.z, outgoingAxial.y, outgoingAxial.x); //swap axis ignoring y
                outgoingAxial *= -1; //invert coordinate
            }
            //add relative mirror origin back into equation since it had been removed.
            outgoingAxial += _mirrorOriginAxial;

            return _mapManager.CastAxialToOddRow(outgoingAxial);
        }

        //if the system mirrors character action instead, coordinate isn't as critical, just mirroring the axial direction and distance amount.
    }

}
