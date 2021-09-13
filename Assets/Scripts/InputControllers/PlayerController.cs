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

    private Vector3 velocity = Vector3.zero;
    private Vector3 facing = Vector3.zero;

    private Bow bow;

    private Vector2 movement = Vector2.zero;

    private Vector3 mousePos = Vector3.zero;

    private CharacterController controller;

    private Rigidbody rigidbody;

    private void Awake()
    {
        animator.SetBool("isMoving", false);
        bow = GetComponentInChildren<Bow>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Am i local?: " + isLocalPlayer.ToString());
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
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out var hit);
        facing = Vector3.Normalize(hit.point - transform.position);
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
