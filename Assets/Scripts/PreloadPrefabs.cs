using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PreloadPrefabs : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnLoad() {
        var prefabs = Resources.LoadAll<GameObject>("Preload"); // Located in Assets/Resources/Preload
        if (prefabs != null) {
            Debug.Log(prefabs.Length + " Preload Assets Found!");
            foreach (var prefab in prefabs) {
                if (NetworkServer.active) {
                    NetworkServer.Spawn(Instantiate(prefab));
                    Debug.Log("Preloaded (Local + Server): " + prefab.name);
                } else {
                    Instantiate(prefab);
                    Debug.Log("Preloaded (Local): " + prefab.name);
                }
                
            }
        } else {
            Debug.Log("No Preload Assets Found!");
        }
    }
}
