using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameJam
{
    public class ReferenceManager : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        public Tilemap Tilemap => _tilemap;
        
    }
}
