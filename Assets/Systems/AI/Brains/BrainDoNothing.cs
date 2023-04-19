using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;

namespace GameJam.Entity.Brain
{
    [System.Serializable]
    public class BrainDoNothing : BrainBase
    {
        private MapInteractionManager _mapInteractionManager;
        private EntityBase _entityBase;
        private void Awake()
        {
            _entityBase = GetComponent<EntityBase>();
        }
        private void Start() {
            _mapInteractionManager = GameMaster.Instance.ReferenceManager.MapInteractionManager;
        }
        public override void Think()
        {
            _entityBase.ActionCompleted();
        }
        public override void TelegraphNextTurn()
        {
        }
    }
}
