using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity
{
    public class EntityCharacter : EntityBase
    {
        

        public override void DoTurnAction()
        {
            CompletedTurn();
        }
    }
}
