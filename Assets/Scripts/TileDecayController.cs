using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDecayController : MonoBehaviour
{
    [SerializeField]
    private float decayRate;
    [SerializeField]
    private float decayAmount;
    private float decayTimer;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        decayTimer = decayRate;
    }

    // Update is called once per frame
    void Update()
    {
        decayTimer -= Time.deltaTime;
        if (decayTimer < 0)
        {
            active = true;
        }
    }

     //Removes a random amount of tiles with a probability determined for each tile.
    public void RemoveTiles(List<Tile> floorTiles)
    {
        Debug.Log("Decaying Stage");
        decayTimer = decayRate;
        active = false;
        List<Tile> deleteQueue = new List<Tile>();
        foreach (Tile t in floorTiles)
        {
            int r = Random.Range(0, 5);
            // Chance of being removed is higher for tiles with less neighbours (there is a 4-neighbour_count/4 chance that the tile will be removed)
            if (t.neighbours.Count < r)
            {
                deleteQueue.Add(t);
            }
        }
        for (int i = 0; i < deleteQueue.Count; i++)
        {
            floorTiles.Remove(deleteQueue[i]);
            deleteQueue[i].Delete();
        }
    }
}
