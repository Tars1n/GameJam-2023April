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
        public List<Activity> ActivitiesToLoop { get => _activitiesToLoop;}
        
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
        [SerializeField] private GameObject _telegraphGO;
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
        public void SetActivitiesToLoop(List<Activity> activitiesToSet)
        {
            //I need to manually re-create the list, otherwise it just stores a reference.
            if ((activitiesToSet == null) || (activitiesToSet.Count == 0)) return;
            _activitiesToLoop = new List<Activity>();
            foreach (Activity activity in activitiesToSet)
            {
                _activitiesToLoop.Add(activity);
            }
        }
        public override void Think()
        {
            _telegraphGO.SetActive(false);
            TileNode tile = NextStepTile();
            if (_mapInteractionManager.TryToTakeAction(_entityBase, tile) == false)
                _mapInteractionManager.HopEntity(_entityBase, _entityBase?.CurrentTileNode, 0);
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
            TileNode tile = NextStepTile();
            if (tile == null) { return; }
            _telegraphGO.transform.position = tile.WorldPos;
            _telegraphGO.SetActive(true);
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
            Gizmos.color = _gizmoColour;
            Vector3Int previousCoord = Map.Map.WorldToCell(transform.position);
            Vector3 previousPoint = Map.Map.CellToWorld(previousCoord);
            Vector3Int previousAxial = Map.CastOddRowToAxial(previousCoord);
            foreach (Activity activity in _activitiesToLoop)
            {
                Vector3Int axialCoord = activity.GridCoord + previousAxial;
                previousCoord = Map.CastAxialToOddRow(axialCoord);
                Vector3 position = Map.GetWorldPosFromGridCoord(previousCoord);
                
                Gizmos.DrawLine(previousPoint, position);
                previousAxial = axialCoord;
                previousPoint = position;
            }
        }
    }
}
