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
    public Animator animator;

    private MouseWorld mouseWorld;
    private Knockback force;

    [SerializeField]
    private float dashMultiplier = 5.0f;

    [SerializeField]
    private float dashTimer = 0.5f;

    [SerializeField]
    private float dashCooldown = 1.0f;

    [SerializeField]
    private float currDashCooldown = 0f;

    private float startingTime;

    // debug
    public bool isMoving = false;

    // debug set
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }

    public PlayerMovingState moveState { get; private set; }

    public PlayerDashingState dashState { get; private set; }

    public PlayerFallingState fallState { get; private set; }

    private Vector3 velocity = Vector3.zero;

    private Vector3 startingVelocity = Vector3.zero;

    /*debug*/
    public Vector3 myVelocity; 

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

            // state machine test
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMovingState(this, stateMachine, "move");
        dashState = new PlayerDashingState(this, stateMachine, "dash");
        fallState = new PlayerFallingState(this, stateMachine, "fall");
        stateMachine.Initialize(idleState);
    }

    private string[] outlineColours = new string[]{"Red", "Green", "Blue", "Purple"};

    public override void OnStartLocalPlayer()
    {
        // transform.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer(outlineColours[NetworkManager.singleton.numPlayers - 1]);
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

            // dash cd 
        currDashCooldown -= Time.deltaTime;
        if (currDashCooldown < 0)
        {
            currDashCooldown = 0;
        }
        myVelocity = rigidbody.velocity;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        
        if (context.performed)
        {
            animator.SetBool("isMoving", true);
            isMoving = true;
        }
            
        if (context.canceled)
        {
            animator.SetBool("isMoving", false);
            isMoving = false;
        }
            
        movement = context.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!isLocalPlayer) return;
        if (context.started)
        {
            //Debug.Log("Dashing");
            
            // Use velocity with dash time, don't use add force... this will caus some ramp slides
            if (currDashCooldown <= 0 && isMoving)
            {
                Debug.Log("DASH");
                StartCoroutine(Dash());
                currDashCooldown += dashCooldown;
            }
            // rigidbody.AddForce((movement * 80), ForceMode.Impulse);
        }
            
    }
        
    IEnumerator Dash()
    {
        startingVelocity = new Vector3(movement.x * speed, 0, movement.y * speed);
        startingTime = Time.time;
        while(Time.time < startingTime + dashTimer)
        {
            transform.Translate(startingVelocity * dashMultiplier * Time.deltaTime, Space.World);
            yield return null;
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
