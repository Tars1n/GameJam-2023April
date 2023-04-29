using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Entity;
using GameJam.Map;

namespace GameJam.Dialogue
{
    [System.Serializable]
    public class DialoguePieceHopEntityClass : DialoguePieceClass
    {
        [SerializeField] private EntityBase _entity;
        public EntityBase Entity => _entity;
        [SerializeField] private Vector3Int _tileToHopTo;
        public Vector3Int TileToHopTo => _tileToHopTo;
        [SerializeField] private float _durationOfHop = 0.5f;
        public float DurationOfHop => _durationOfHop;
        [SerializeField] private bool _slamAtEnd;
        public bool SlamAtEnd => _slamAtEnd;

        public override void DoPiece(DialogueManager dialogueManager)
        {
            DoHopEntity();
        }

        private void DoHopEntity()
        {
            TileNode tileNode = _ref.TileNodeManager.GetTileFromAxial(_tileToHopTo);

            _ref.MapInteractionManager.HopEntityToPosFunc(_entity, tileNode, _durationOfHop, _slamAtEnd);
            _entity.OnEntityStoppedMoving += ProgressCutscene;
        }

        public void ProgressCutscene()
        {
            _entity.OnEntityStoppedMoving -= ProgressCutscene;
            _ref.DialogueManager.NextDialoguePiece();
        }
    }
}
