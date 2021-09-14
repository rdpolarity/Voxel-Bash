using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Arrow : NetworkBehaviour
{
    public LayerMask bounceMask;

    private Rigidbody rigidbody;
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
            if (rigidbody != null) {                
                Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
                rigidbody.MoveRotation(targetRotation);
            }

            if (transform.position.y < -100)
            {
                Destroy(gameObject);
            }

            float speed = (transform.position - lastPos).magnitude;
            collisionDetect = new Ray(transform.position, rigidbody.velocity.normalized);
            RaycastHit hit;
            if (Physics.Raycast(collisionDetect, out hit, 0.5f, bounceMask))
            {
                Debug.Log("Ricochet");
                rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, hit.normal);
                rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            }

            lastPos = transform.position;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkServer.active) {
            MapManager.Instance.DestroyMapBlock(collision.gameObject);
            if (collision.gameObject.CompareTag("Tile"))
            {
                NetworkServer.Destroy(gameObject);
                Destroy(gameObject);
            }
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit player");
                collision.gameObject.GetComponent<Knockback>().AddImpact(rigidbody.velocity.normalized, Mathf.Min(20, rigidbody.velocity.x+rigidbody.velocity.y));
                Destroy(gameObject);
            }
        }
        
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(collisionDetect);
    }
}
