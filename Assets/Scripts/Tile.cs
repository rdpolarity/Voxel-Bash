using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 coordinates;
    public List<Tile> neighbours;
    
    //IMPORTANT!!!! this method must be used to destroy a tile to prevent null fields in other lists
    public void Delete()
    {
        foreach (Tile t in neighbours)
        {
            t.neighbours.Remove(this);
        }
        Destroy(gameObject);
    }
}
