using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

/// <summary>
/// This class will add players to the camera track group
/// </summary>
public class PlayerCameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitCamera());
    }

    IEnumerator InitCamera() {
        yield return new WaitForSeconds(1);
        var targetGroup = GetComponent<CinemachineTargetGroup>();
        foreach(var player in GameObject.FindGameObjectsWithTag("Player")) {
            Debug.Log(player.name);
            targetGroup.AddMember(player.gameObject.transform, 1, 0);
        }
    }
}
