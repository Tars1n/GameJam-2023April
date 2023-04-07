using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    [RequireComponent(typeof(EntityManager))]
    public class TurnManager : MonoBehaviour
    {   
        private EntityManager _entityManager;
        public EntityManager EntityManager => _entityManager;
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
            }
        }

        private void SetupNewRound()
        {
            //calls this at the start of game and anytime computer has no actions left to play.
            //set all entities to have an action
            _round ++;
            _entityManager.SetAllEntitiesToHaveActionReady();
            _playerTurn = true;
            Debug.Log($"===================  Start of Round {_round}  ===================");
            
            StartPlayerTurn();
        }

        private void StartPlayerTurn()
        {
            _playerTurn = true;
        }

        private void StartComputerTurn()
        {
            _playerTurn = false;
            TickNext();
        }

        public void ActionCompleted()
        {
            //Entities call this when they've completed their action, this causes turn manager to progress to the next Actor
            if (GameMaster.Instance.GameSuspended)
            {
                Debug.Log("Cannot pursue next action as game is Suspended.");
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
                Debug.Log("All entities have completed their turns, starting next round.");
                SetupNewRound();
                return;
            }
            
            nextEntity.DoTurnAction();
        }

        private void TryEndPlayerTurn()
        {
            if (_entityManager.DoesPlayerStillHaveAction())
                return;
            
            StartComputerTurn();
        }
    }
}
