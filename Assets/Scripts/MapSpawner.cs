using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{   
    [SerializeField]
    private GameObject gameObject;

    void Start() {

        // Spawn the map in
        Instantiate(GameSettings.Instance.SelectedMap, transform.position, Quaternion.identity);
        
    }
}
