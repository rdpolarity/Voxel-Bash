using Mirror;
using UnityEngine;

namespace RDPolarity.Basic
{
    /// <summary>
    /// Manages the lobby ready zone logic
    /// </summary>
    public class ReadyZone : MonoBehaviour
    {
        void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player"))
            {
                NetworkManager.singleton.ServerChangeScene("Arena");
            }
        }
    }
}
