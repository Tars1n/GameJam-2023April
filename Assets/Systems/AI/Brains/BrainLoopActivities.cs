using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity;

namespace GameJam.Entity.Brain
{
    [System.Serializable]
    public class BrainLoopActivities : BrainBase
    {
        [SerializeField] private List<Activity> _activitiesToLoop;
        public List<Activity> ActivitiesToLoop => _activitiesToLoop;
        //Axial inputs to move one step
        //Up left: -1, 1, 0
        //left: -1, 0, 1
        //down left: 0, -1, 1
        //down right: 1, -1, 0
        //right: 1, 0, -1
        //up right: 0, 1, -1
        [SerializeField] private int _stepInActivityLoop;
        [SerializeField] private Color _gizmoColour;
        private MapManager _mapManager;
        private MapManager Map => _mapManager ? _mapManager : _mapManager = GameObject.Find("Tilemap").GetComponent<MapManager>();
        private ReferenceManager _ref;
        private EntityBase _entityBase;
        private MapInteractionManager _mapInteractionManager;
        private TileNodeManager _tileNodeManager;
        private Vector3Int _telegraphingCoord;
        private void Awake()
        {  
            _entityBase = GetComponent<EntityBase>();      
        }
        private void Start() 
        {
            _ref = GameMaster.Instance.ReferenceManager;
            _mapManager = _ref.MapManager;
            _mapInteractionManager = _ref.MapInteractionManager;
            _tileNodeManager = _ref.TileNodeManager;
        }
        public override void Think()
        {
            _mapManager.TriggerTilemap.SetTile(_telegraphingCoord, null);
            TileNode tile = NextStepTile();
            _mapInteractionManager.TryToTakeAction(_entityBase, tile);
            IncreaseStep();
        }

        private TileNode NextStepTile()
        {
            Vector3Int axialToMoveTo = _mapManager.CastOddRowToAxial(_entityBase.CurrentTileNode.GridCoordinate);
            axialToMoveTo += _activitiesToLoop[_stepInActivityLoop].GridCoord;
            // Debug.Log($"move to coords " + axialToMoveTo);
            TileNode tile = _tileNodeManager.GetNodeFromCoords(_mapManager.CastAxialToOddRow(axialToMoveTo));
            return tile;
        }

        public override void TelegraphNextTurn()
        {
            _mapManager.TriggerTilemap.SetTile(_telegraphingCoord, null);
            TileNode tile = NextStepTile();
            if (tile == null) { return; }
            _telegraphingCoord = tile.GridCoordinate;
            _mapManager.TriggerTilemap.SetTile(_telegraphingCoord, _mapInteractionManager?.MonsterTelegraph);
        }

        private void IncreaseStep()
        {
            _stepInActivityLoop ++;
            if (_stepInActivityLoop >= _activitiesToLoop.Count)
            {
                _stepInActivityLoop = 0;
            }
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = _gizmoColour;
            Vector3 previousPoint = this.transform.position;
            foreach (Activity activity in _activitiesToLoop)
            {
                Vector3Int coord = activity.GridCoord + Map.Map.WorldToCell(transform.position);
                Vector3 position = Map.GetWorldPosFromGridCoord(coord);
                
                Gizmos.DrawLine(previousPoint, position);
                previousPoint = position;
            }
        }
    }
}
