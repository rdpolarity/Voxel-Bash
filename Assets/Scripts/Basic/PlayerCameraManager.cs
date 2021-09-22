using System.Collections;
using Cinemachine;
using UnityEngine;

namespace RDPolarity.Basic
{
    /// <summary>
    /// This class will add players to the camera track group
    /// </summary>
    public class PlayerCameraManager : MonoBehaviour
    {
        public void InitCamera() {
            var targetGroup = GetComponent<CinemachineTargetGroup>();
            foreach(var player in GameObject.FindGameObjectsWithTag("Player")) {
                Debug.Log(player.name);
                targetGroup.AddMember(player.gameObject.transform, 1, 0);
            }
        }
    }
}
