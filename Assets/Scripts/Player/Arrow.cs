using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Arrow : NetworkBehaviour
{
    private Rigidbody rigidbody;

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
            Quaternion targetRotation = Quaternion.LookRotation(rigidbody.velocity);
            rigidbody.MoveRotation(targetRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NetworkServer.active) {
            MapManager.Instance.DestroyMapBlock(collision.gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}
