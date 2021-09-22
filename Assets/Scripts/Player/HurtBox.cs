using RDPolarity.Controllers;
using UnityEngine;

namespace RDPolarity.Player
{
    public class HurtBox : MonoBehaviour
    {
        PlayerController _playerController;

        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
        }

    
    }
}
