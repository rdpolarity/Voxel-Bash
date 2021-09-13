using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            VoxelBashNetworkManager.singleton.ServerChangeScene("Arena");
        }
    }
}
