using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    void Start() {
        Instantiate(GameSettings.Instance.SelectedMap, transform.position, Quaternion.identity);
    }
}
