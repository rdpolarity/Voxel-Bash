using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.VFX;

public class Tile : MonoBehaviour
{
    public GameObject nestedBlock;
    public GameObject explosionEffect;
    public Vector2 coordinates;
    public List<Tile> neighbours;
    
    //IMPORTANT!!!! this method must be used to destroy a tile to prevent null fields in other lists
    public void Delete()
    {
        Debug.Log("Destroyed");
        if (explosionEffect != null) Instantiate(explosionEffect, transform.position, transform.rotation);
        foreach (Tile t in neighbours)
        {
            t.neighbours.Remove(this);
        }
        if (nestedBlock != null)
        {
            nestedBlock.transform.localPosition = new Vector3(0, 20, 0);
        }
        gameObject.transform.position = gameObject.transform.position - new Vector3(0, 20, 0);
    }
}
