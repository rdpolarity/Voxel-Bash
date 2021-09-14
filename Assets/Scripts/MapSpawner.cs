using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Will Spawn the selected map from [MapManager] on start
/// </summary>
public class MapSpawner : MonoBehaviour
{   
    void Start() {
        if (NetworkServer.active) {
            MapManager.Instance.SpawnMap(transform.position);
        }   
    }
}
