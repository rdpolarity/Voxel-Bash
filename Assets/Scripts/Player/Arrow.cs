using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public LayerMask bounceMask;

    private Rigidbody rigidbody;
    private Ray collisionDetect;
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
        rigidbody.MoveRotation(targetRotation);

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player");
            collision.gameObject.GetComponent<Knockback>().AddImpact(rigidbody.velocity.normalized, Mathf.Min(20, rigidbody.velocity.x+rigidbody.velocity.y));
            Destroy(gameObject);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(collisionDetect);
    }
}
