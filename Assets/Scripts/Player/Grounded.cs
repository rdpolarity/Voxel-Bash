using UnityEngine;

namespace RDPolarity.Player
{
    public class Grounded : MonoBehaviour
    {
        public bool active;
        [SerializeField]
        private int tilecount;

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Indestructable"))
            {
                tilecount += 1;
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if(collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Indestructable"))
            {
                tilecount -= 1;
            }
        }

        private void Update()
        {
            if (tilecount > 0)
            {
                active = true;
            }
            else
            {
                active = false;
            }
        }
    }
}
