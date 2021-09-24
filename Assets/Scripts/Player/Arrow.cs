using System;
using Mirror;
using RDPolarity.Arena;
using RDPolarity.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace RDPolarity.Player
{
    public class Arrow : NetworkBehaviour
    {
        public LayerMask bounceMask;

        private new Rigidbody rigidbody;
        private Ray collisionDetect;
        private Vector3 lastPos;

        [SerializeField]
        private float maxTiles;
        [SerializeField] private GameObject onHitParticles;
        private float tilesHit;

        private Quaternion targetRotation;
        
                
        [Serializable] public class OnHitEvent : UnityEvent { }
        public OnHitEvent onHitEvent = new OnHitEvent();
        
        [Serializable] public class OnHitSelfEvent : UnityEvent { }
        public OnHitSelfEvent onHitSelfEvent = new OnHitSelfEvent();
        
        [Serializable] public class OnHitOthersEvent : UnityEvent { }
        public OnHitOthersEvent onHitOthersEvent = new OnHitOthersEvent();
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            rigidbody = GetComponent<Rigidbody>();
            if (!NetworkServer.active) {
                Destroy(rigidbody);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (NetworkServer.active) {
                if (transform.position.y < -100)
                {
                    NetworkServer.Destroy(gameObject);
                }

                if (rigidbody != null) {                
                    Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
                    rigidbody.MoveRotation(targetRotation);
                    float speed = (transform.position - lastPos).magnitude;
                    collisionDetect = new Ray(transform.position, rigidbody.velocity.normalized);
                    RaycastHit hit;
                    if (Physics.Raycast(collisionDetect, out hit, 0.5f, bounceMask))
                    {
                        Debug.Log("Ricochet");
                        rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, hit.normal);
                        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
                    }
                }

                lastPos = transform.position;
            }
        
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (NetworkServer.active) {
                if (collision.gameObject.CompareTag("Tile"))
                {
                    if (tilesHit < maxTiles)
                    {
                        tilesHit++;
                        collision.gameObject.GetComponent<Block>().Delete();
                        if (tilesHit >= maxTiles)
                        {
                            NetworkServer.Destroy(gameObject);
                        }
                    }
                
                }
                else if (collision.gameObject.CompareTag("Indestructable")) {
                    NetworkServer.Destroy(gameObject);
                }
            }
        
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit player");
                onHitEvent.Invoke();;
                if (!isLocalPlayer) onHitOthersEvent.Invoke();;
                Instantiate(onHitParticles, transform.position, transform.rotation);
                other.GetComponentInParent<Rigidbody>().AddForce(rigidbody.velocity*2, ForceMode.Impulse);
                NetworkServer.Destroy(gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(collisionDetect);
        }
    }
}
