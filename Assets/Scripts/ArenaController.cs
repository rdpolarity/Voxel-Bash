using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    private ArenaGenerator generator;
    private TileDecayController decay;

    [SerializeField]
    private List<Tile> floorTiles;

    

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<ArenaGenerator>();
        decay = GetComponent<TileDecayController>();
        floorTiles = generator.Generate();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Removes tiles that have been deleted from other places in the code
        floorTiles.RemoveAll(item => item == null);
        if (decay.active)
        {
            decay.RemoveTiles(floorTiles);
        }
    }
}
