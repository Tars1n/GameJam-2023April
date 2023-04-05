using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public abstract class EntityBase : MonoBehaviour
    {
        [SerializeField] private Map.TileNode _currentTileNode;
        protected SpriteRenderer _spriteRenderer;
        public bool HasActedThisRound = false;

        protected virtual void Start()
        {
            GameMaster.Instance.ReferenceManager.EntityManager.AddEntity(this);
        }
        public abstract void DoTurnAction();
    }
}
