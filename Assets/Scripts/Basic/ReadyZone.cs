using Mirror;
using RDPolarity.Controllers;
using UnityEngine;

namespace RDPolarity.Basic
{
    /// <summary>
    /// Manages the lobby ready zone logic
    /// </summary>
    public class ReadyZone : NetworkBehaviour
    {
        void OnTriggerEnter(Collider other) {
            if (other.TryGetComponent<PlayerController>(out var controller))
            {
                if (NetworkServer.active && other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer) {
                    NetworkManager.singleton.ServerChangeScene("Arena");
                }
            }
        }
    }
}
