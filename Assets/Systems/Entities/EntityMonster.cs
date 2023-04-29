using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Map;
using GameJam.Entity.Brain;

namespace GameJam.Entity
{
    public class EntityMonster : EntityBase
    {
        //monster brain
        [SerializeField] private MonsterBlueprint _monsterBlueprint;
        private KillAdjacentPlayer _killAdjacentPlayer;
        // something that represents goal.
        private BrainBase _brain;
        
        private void Awake()
        {
            _brain = GetComponent<BrainBase>();
            _killAdjacentPlayer = GetComponent<KillAdjacentPlayer>();
        }

        public override void SetupEntity()
        {
            base.SetupEntity();
        }

        public override void RefreshAction()
        {
            base.RefreshAction();
            _brain.TelegraphNextTurn();
        }
        public override void DoTurnAction()
        {
            _killAdjacentPlayer?.KillIfCan();
            _brain?.Think();
        }     
        public override void FallInPit()
        {
            base.FallInPit();
            if (IsCurrentlyMoving == false)
            {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.Lib.MonsterFallIntoPit);
            }
            _dialogueManager.DoDialogue(_fallInPitDialogue);
        }   
        public override void TriggerTrap()
        {
            base.TriggerTrap();
            _dialogueManager.DoDialogue(_triggersTrap);
        }
    }
}
