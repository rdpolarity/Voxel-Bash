using System;
using System.Collections;
using Mirror;
using RDPolarity.Arena;
using RDPolarity.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace RDPolarity.Player
{
    public class Arrow : NetworkBehaviour
    {
        [SerializeField] private float maxTiles;
        [SerializeField] private GameObject onHitParticles;
        [SerializeField] private Rigidbody _rigidbody;

        public LayerMask bounceMask;

        private Ray collisionDetect;
        private Vector3 lastPos;
        private float tilesHit;


        private Quaternion targetRotation;

        // Update is called once per frame
        void FixedUpdate()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_rigidbody.velocity);
            _rigidbody.MoveRotation(targetRotation);

            collisionDetect = new Ray(transform.position, _rigidbody.velocity.normalized);
            RaycastHit hit;
            if (Physics.Raycast(collisionDetect, out hit, 0.5f, bounceMask))
            {
                Debug.Log("Ricochet");
                _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, hit.normal);
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            }

            // if (NetworkServer.active) {
            //     if (transform.position.y < -100)
            //     {
            //         NetworkServer.Destroy(gameObject);
            //     }
            //
            //     if (rigidbody != null) {                
            //         Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
            //         rigidbody.MoveRotation(targetRotation);
            //         float speed = (transform.position - lastPos).magnitude;
            //     }
            //
            //     lastPos = transform.position;
            // }
        }


        /// <summary>
        /// Launches the arrow in a direction based on the power
        /// </summary>
        /// <param name="power"></param>
        public void Launch(Vector3 power)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(power, ForceMode.Impulse);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Tile"))
            {
                if (tilesHit < maxTiles)
                {
                    tilesHit++;
                    collision.gameObject.GetComponent<Block>().Delete();
                    if (tilesHit >= maxTiles)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (collision.gameObject.CompareTag("Indestructable"))
            {
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(collisionDetect);
        }
    }
}