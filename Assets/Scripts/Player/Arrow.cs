using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Arrow : NetworkBehaviour
{
    public LayerMask bounceMask;

    private new Rigidbody rigidbody;
    private Ray collisionDetect;
    private Vector3 lastPos;

    private Quaternion targetRotation;

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
                collision.gameObject.transform.position = collision.gameObject.transform.position - new Vector3(0,20, 0);
                NetworkServer.Destroy(gameObject);
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
            other.gameObject.GetComponent<Knockback>().AddImpact(rigidbody.velocity.normalized, Mathf.Min(30, rigidbody.velocity.x + rigidbody.velocity.y));
            NetworkServer.Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(collisionDetect);
    }
}
