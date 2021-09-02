using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ArenaGenerator : MonoBehaviour
{

    [SerializeField, Range(0.0f, 10.0f)]
    private int width = 1, height = 1;

    [SerializeField]
    private GameObject floor;

    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {            
                Gizmos.DrawWireCube(new Vector3(w, 0, h), new Vector3(1, 1, 1));
            }
        }
    }

    [Button]
    public List<Tile> Generate() {
        var floorGroup = new GameObject("FloorGroup");
        List<Tile> floorTiles = new List<Tile>();
        floorGroup.transform.parent = gameObject.transform;
        Gizmos.color = Color.yellow;
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {            
                var floorObject = Instantiate(floor);
                floorObject.transform.position = new Vector3(w, 0, h);
                floorObject.transform.parent = floorGroup.transform;
                Tile t = floorObject.GetComponent<Tile>();
                t.coordinates = new Vector2(w, h);
                floorTiles.Add(t);
            }
        }
        foreach (Tile t in floorTiles)
        {
            int index = (int)((t.coordinates.y * width) + t.coordinates.x);
            if (index - 1 > 0)
            {
                t.neighbours.Add(floorTiles[index - 1]);
            }
            if (index + 1 < width * height)
            {
                t.neighbours.Add(floorTiles[index + 1]);
            }
            if (index - width > 0)
            {
                t.neighbours.Add(floorTiles[index - width]);
            }
            if (index + width < width * height)
            {
                t.neighbours.Add(floorTiles[index + width]);
            }
            
        }

        return floorTiles;
    }
}
