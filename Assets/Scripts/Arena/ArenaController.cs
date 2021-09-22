using System.Collections.Generic;
using UnityEngine;

namespace RDPolarity.Arena
{
    /// <summary>
    /// Randomly removes blocks from the arena
    /// TODO: Not needed or used currently
    /// </summary>
    public class ArenaController : MonoBehaviour
    {
        [SerializeField] private List<Block> floorTiles;
        
        private ArenaGenerator _generator;
        private TileDecayController _decay;
        
        void Start()
        {
            _generator = GetComponent<ArenaGenerator>();
            _decay = GetComponent<TileDecayController>();
            floorTiles = _generator.Generate();
        
        }

        void Update()
        {
            //Removes tiles that have been deleted from other places in the code
            floorTiles.RemoveAll(item => item == null);
            if (_decay.active)
            {
                _decay.RemoveTiles(floorTiles);
            }
        }
    }
}
