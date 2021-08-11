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
    private void Generate() {
        var floorGroup = new GameObject("FloorGroup");
        floorGroup.transform.parent = gameObject.transform;
        Gizmos.color = Color.yellow;
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {            
                var floorObject = Instantiate(floor);
                floorObject.transform.position = new Vector3(w, 0, h);
                floorObject.transform.parent = floorGroup.transform;
            }
        }
    }
}
