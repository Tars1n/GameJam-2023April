using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam.Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class TurnManager : MonoBehaviour
    {   
        public bool DebugLog = true;
        private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;
        [SerializeField] private float _delayBetweenActions = 0.5f;
        public float DelayBetweenActions => _delayBetweenActions;
        [SerializeField] private bool _playerTurn;
        public bool PlayerTurn => _playerTurn;
        [SerializeField] private int _round = 0;
        public int Round => _round;
        
        

        private void Awake()
        {
            _entityManager = GetComponent<EntityManager>();    
        }

        public void BeginPlay()
        {
            if (_round == 0)
            {
                SetupNewRound();
                GameMaster.Instance.TilemapInteractable = true;
            }
        }

        private void SetupNewRound()
        {
            //calls this at the start of game and anytime computer has no actions left to play.
            //set all entities to have an action
            GameMaster.Instance.SetActiveEntity(null);

            _round ++;
            
            _entityManager.QueueActionForAllEntities();
            _playerTurn = true;
            
            if (DebugLog)  Debug.Log($"===================  Start of Round {_round}  ===================");
            
            StartPlayerTurn();
        }

        private void StartPlayerTurn()
        {
            _playerTurn = true;
            if (GameMaster.Instance.MultiplePlayerCharacters == false)
            {
                //If game set to single player character, select by default, otherwise start turn with no selection.
                EntityCharacter player = _entityManager.PlayerCharacters[0];
                GameMaster.Instance.SetActiveEntity(player);
            }
            if (DebugLog)  Debug.Log("Player's turn begins.");
        }

        private void StartComputerTurn()
        {
            if (DebugLog)  Debug.Log("Player has completed their turn. Evil begins to stir.");
            _playerTurn = false;
            _entityManager.EnqueueAllMapEntities();
            TickNext();
        }

        public void ActionCompleted()
        {
            GameMaster.Instance.SetActiveEntity(null);
            //Entities call this when they've completed their action, this causes turn manager to progress to the next Actor
            if (GameMaster.Instance.GameSuspended)
            {
                if (DebugLog)  Debug.Log("Cannot pursue next action as game is Suspended.");
                return;
            }

            TickNext();
        }

        public void TickNext()
        {
            if (_playerTurn)
            {
                TryEndPlayerTurn();
                return;
            }
            EntityBase nextEntity = _entityManager.GetNextReadyMapEntity();
            if (nextEntity == null)
            {
                if (DebugLog)  Debug.Log("All entities have completed their turns, starting next round.");
                SetupNewRound();
                return;
            }
            //if (DebugLog) Debug.Log($"Next Actor: {nextEntity}");
            GameMaster.Instance.SetActiveEntity(nextEntity);
            nextEntity.DoTurnAction();
        }

        private void TryEndPlayerTurn()
        {
            if (_entityManager.DoesPlayerStillHaveAction())
                return;
            
            StartCoroutine("DelayedComputerTurnStart");
        }

        IEnumerator DelayedComputerTurnStart()
        {
            yield return new WaitForSeconds(_delayBetweenActions);
            StartComputerTurn();
        }
    }
}
