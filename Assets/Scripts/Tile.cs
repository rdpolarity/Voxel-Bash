using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 coordinates;
    public List<Tile> neighbours;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //IMPORTANT!!!! this method must be used to destroy a tile to prevent null fields in other lists
    public void Delete()
    {
        foreach (Tile t in neighbours)
        {
            t.neighbours.Remove(this);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow"))
        {
            Delete();
        }
    }
}
