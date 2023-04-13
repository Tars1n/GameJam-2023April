using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam.Entity.Shoving
{
    public class Shovable : MonoBehaviour
    {
        private EntityBase _entityBase;

        private void Awake()
        {
            _entityBase = GetComponent<EntityBase>();
        }
        public void TryShoveDir(Vector3Int axialDir)
        {

        }
    }
}
