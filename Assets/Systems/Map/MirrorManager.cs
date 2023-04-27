using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GameJam.Entity;
using GameJam.Entity.Abilities;
using Sirenix.OdinInspector;

namespace GameJam.Map
{
    public class MirrorManager : MonoBehaviour
    {
        private GameMasterSingleton _gm => GameMaster.Instance;
        private MapManager _mapManager;
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        [SerializeField] private bool _movementControlsAreMirrored;
        [SerializeField] private Vector3Int _mirrorOrigin;
        public Vector3Int MirrorOrigin => _mirrorOrigin;
        private Vector3Int _mirrorOriginAxial;
        [SerializeField] private bool _mirrorX = true;
        public bool MirrorX => _mirrorX;
        [ShowIf("_mirrorX")]
        [SerializeField] private EntityBase _entityLeft;
        public EntityBase EntityLeft => _entityLeft;
        [ShowIf("_mirrorX")]
        [SerializeField] private EntityBase _entityRight;
        public EntityBase EntityRight => _entityRight;
        [SerializeField] private bool _mirrorY = false;
        public bool MirrorY => _mirrorY;
        [ShowIf("_mirrorY")]
        [SerializeField] private EntityBase _entityTop;
        public EntityBase EntityTop => _entityTop;
        [ShowIf("_mirrorY")]
        [SerializeField] private EntityBase _entityBottom;
        public EntityBase EntityBottom => _entityBottom;
        

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

        public void SelectActivePlayer(Vector3Int mouseCoordinate)
        {
            if (_mirrorX)
            {
                if (mouseCoordinate.x < _mirrorOrigin.x && _entityLeft != null)
                {
                    _gm.SetActiveEntity(_entityLeft);
                }
                if (mouseCoordinate.x > _mirrorOrigin.x && _entityRight != null)
                {
                    _gm.SetActiveEntity(_entityRight);
                }
            }
            if (MirrorY)
            {
                if (mouseCoordinate.y < _mirrorOrigin.y && _entityBottom != null)
                {
                    _gm.SetActiveEntity(_entityBottom);
                }
                if (mouseCoordinate.y > _mirrorOrigin.y && _entityTop != null)
                {
                    _gm.SetActiveEntity(_entityTop);
                }
            }
            if (!_mirrorX && !_mirrorY)
            {
                _gm.SetActiveEntity(_gm.ReferenceManager.EntityManager.GetNextActivePlayerCharacter());
            }
        }

        public Vector3Int ReflectGridCoordinate(Vector3Int coord)
        {
            Vector3Int incomingAxial = _mapManager.CastOddRowToAxial(coord);
            Vector3Int outgoingAxial = incomingAxial - _mirrorOriginAxial; //make axis relative to mirror _mirrorOrigin
            if (_mirrorX)
                { outgoingAxial = ReflectAxialX(outgoingAxial); }
            if (_mirrorY)
                { outgoingAxial = ReflectAxialY(outgoingAxial); }

            outgoingAxial += incomingAxial;

            return _mapManager.CastAxialToOddRow(outgoingAxial);
        }

        public Vector3Int ReflectAxialX(Vector3Int axialPointer)
        {
            if (_movementControlsAreMirrored == false)
                { return axialPointer; }
            axialPointer = new Vector3Int(axialPointer.z, axialPointer.y, axialPointer.x); //swap axis ignoring y
            return axialPointer;
        }

        public Vector3Int ReflectAxialY(Vector3Int axialPointer)
        {
            if (_movementControlsAreMirrored == false)
                { return axialPointer; }
            axialPointer = new Vector3Int(axialPointer.z, axialPointer.y, axialPointer.x);
            axialPointer *= -1; //invert coordinate
            return axialPointer;
        }

        public void RenderMirroredSelection(EntityBase originalEntity, TileNode selectedTile)
        {
            if (originalEntity == null) { return; }
            Mirrored mirrored = originalEntity?.GetComponent<Mirrored>();
            if (mirrored == null) { return; }

            Vector3Int originalPointingVector = _mapManager.CalculateAxialPointerBetweenTiles(originalEntity?.CurrentTileNode, selectedTile);

            if (mirrored.MirrorEntityX != null)
            {
                if (mirrored.MirrorEntityX.HasActionReady)
                {
                    Vector3Int targetAxial = ReflectAxialX(originalPointingVector);
                    Vector3Int entityAxial = mirrored.MirrorEntityX.GetAxialPos();
                    targetAxial += entityAxial;
                    TileNode targetTile = _tileNodeManager.GetTileFromAxial(targetAxial);
                    _mapInteractionManager.RenderPlayerActionTile(mirrored.MirrorEntityX, targetTile);
                }
            }
            
            if (mirrored.MirrorEntityY != null)
            {
                if (mirrored.MirrorEntityY.HasActionReady)
                {
                    Vector3Int targetAxial = ReflectAxialY(originalPointingVector);
                    Vector3Int entityAxial = mirrored.MirrorEntityY.GetAxialPos();
                    targetAxial += entityAxial;
                    TileNode targetTile = _tileNodeManager.GetTileFromAxial(targetAxial);
                    _mapInteractionManager.RenderPlayerActionTile(mirrored.MirrorEntityY, targetTile);
                }
            }
        }

        private TileNode GetReflectedTileOfEntity(EntityBase entity, Vector3Int targetAxial)
        {
            Vector3Int entityAxial = entity.GetAxialPos();
            targetAxial += entityAxial;
            return _tileNodeManager.GetTileFromAxial(targetAxial);
        }

        //if the system mirrors character action instead, coordinate isn't as critical, just mirroring the axial direction and distance amount.
        public void TryMirrorEntityAction(EntityBase originalEntity, TileNode selectedTile)
        {
            Mirrored mirrored = originalEntity?.GetComponent<Mirrored>();
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
            TileNode tileNode = GetReflectedTileOfEntity(mirroredEntity, mirroredPointer);
            if (_mapInteractionManager.TryToTakeAction(mirroredEntity, tileNode) == false)
            {
                _mapInteractionManager.HopEntity(mirroredEntity, mirroredEntity?.CurrentTileNode, 0);
            }
            
        }

        

        
    }

}
