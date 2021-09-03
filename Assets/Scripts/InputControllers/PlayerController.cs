using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;

public class PlayerController : NetworkBehaviour
{
    public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });


    [SerializeField]
    private PlayerInput inputs;
    private MouseWorldPosition mouseWorld;

    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private Animator animator;

    private Vector3 velocity = Vector3.zero;
    private Vector3 facing = Vector3.zero;


    private Rigidbody rigidbody;
    private Bow bow;

    private Vector2 movement = Vector2.zero;

    private Vector3 mousePos = Vector3.zero;

    [ServerRpc(RequireOwnership = false)]
    void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
    {
        Position.Value = gameObject.transform.position;
    }


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        bow = GetComponentInChildren<Bow>();
        mouseWorld = GetComponent<MouseWorldPosition>();
    }

    private void FixedUpdate()
    {
        if (IsLocalPlayer) {            
            velocity = new Vector3(movement.x * speed, 0, movement.y * speed);
            rigidbody.AddForce(velocity, ForceMode.Impulse);
            velocity = Vector3.zero;

            // Falling Velocity
            if (rigidbody.velocity.y < 0f)
            {
                rigidbody.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;
            }

            // Max Velocity
            Vector3 horizontalVelocity = rigidbody.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                rigidbody.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rigidbody.velocity.y;
            }

            // Look Direction (Y)
            //var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            //Physics.Raycast(ray, out var hit);

            facing = Vector3.Normalize(mouseWorld.Position - transform.position);
            facing.y = 0;
            var rotation = Quaternion.LookRotation(facing);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
            bow.Dir = facing;
            SubmitPositionRequestServerRpc();
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer) {              
            if (context.performed)
                animator.SetBool("isMoving", true);
            if (context.canceled)
                animator.SetBool("isMoving", false);
            movement = context.ReadValue<Vector2>();
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (IsLocalPlayer) {         
            if (context.started)
            {
                Debug.Log("Charging");
                bow.Charging = true;

            }
            if (context.canceled)
            {
                Debug.Log("Release");
                bow.Shoot(transform.position);
            }
        }
    }
}
