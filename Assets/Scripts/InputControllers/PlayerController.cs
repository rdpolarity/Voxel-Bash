using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{

    [SerializeField]
    private PlayerInput inputs;

    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private Animator animator;

    private MouseWorld mouseWorld;
    private Knockback force;

    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    private Vector3 facing = Vector3.zero;

    private Bow bow;

    private Vector2 movement = Vector2.zero;

    private Vector3 mousePos = Vector3.zero;

    private CharacterController controller;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        animator.SetBool("isMoving", false);
        bow = GetComponentInChildren<Bow>();
        rigidbody = GetComponent<Rigidbody>();
        mouseWorld = GetComponent<MouseWorld>();
        force = GetComponent<Knockback>();
    }

    private string[] outlineColours = new string[]{"Red", "Green", "Blue", "Purple"};

    public override void OnStartLocalPlayer()
    {
        transform.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer(outlineColours[NetworkManager.singleton.numPlayers - 1]);
        if (isLocalPlayer) {
            inputs.enabled = true;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        velocity = new Vector3(movement.x * speed, 0, movement.y * speed);
        rigidbody.AddForce(velocity, ForceMode.Impulse);
        velocity = Vector3.zero;

        // Max Velocity
        Vector3 horizontalVelocity = rigidbody.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rigidbody.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rigidbody.velocity.y;
        }

        // Look Direction (Y)
        facing = Vector3.Normalize(mouseWorld.Position - transform.position);
        facing.y = 0;
        var rotation = Quaternion.LookRotation(facing);
        var slowRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
        transform.rotation = slowRotation;
        bow.Dir = facing;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.performed)
            animator.SetBool("isMoving", true);
        if (context.canceled)
            animator.SetBool("isMoving", false);
        movement = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.started)
        {
            Debug.Log("Dashing");

            // Use velocity with dash time, don't use add force... this will caus some ramp slides

            // rigidbody.AddForce((movement * 80), ForceMode.Impulse);
        }
            
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.started)
        {
            Debug.Log("Charging");
            bow.Charging = true;

        }
        if (context.canceled)
        {
            Debug.Log("Release");
            bow.shoot(transform.position);
        }
    }

    [SerializeField]
    private GameObject arrow;

    [Command]
    public void CmdShoot(Vector3 pos, Vector3 dir, Vector3 pow)
    {
        var projectile = Instantiate(arrow);
        projectile.transform.position = bow.transform.position + dir;
        projectile.GetComponent<Rigidbody>().velocity = pow;
        NetworkServer.Spawn(projectile);
    }
}
