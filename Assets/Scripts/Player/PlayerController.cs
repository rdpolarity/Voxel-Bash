using System.Collections;
using Mirror;
using RDPolarity.Player;
using RDPolarity.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RDPolarity.Controllers
{
    /// <summary>
    /// Master class for controlling player and related logic
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        // Public Serialized
        [SerializeField] private PlayerInput inputs;
        [SerializeField] private float speed = 1f;
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] public Animator animator;
        [SerializeField] private Grounded grounded;
        [SerializeField] private float dashMultiplier = 5.0f;
        [SerializeField] private float dashTimer = 0.5f;
        [SerializeField] private float dashCooldown = 1.0f;
        [SerializeField] private float currDashCooldown = 0f;
        [SerializeField] private Vector3 myVelocity;
        [SerializeField] private bool isMoving = false;
        [SerializeField] private Vector3 facing = Vector3.zero;
        [SerializeField] private GameObject spawnableBuildBlock;
        [SerializeField] private GameObject arrow;

        // Properties
        public bool IsMoving => isMoving;
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMovingState MoveState { get; private set; }
        public PlayerDashingState DashState { get; private set; }
        public PlayerFallingState FallState { get; private set; }

        // Local Variables
        private MouseWorld _mouseWorld;
        private Knockback _force;
        private float _startingTime;
        private bool _isBuilding = false;
        private Vector3 _velocity = Vector3.zero;
        private Vector3 _startingVelocity = Vector3.zero;
        private Bow _bow;
        private Vector2 _movement = Vector2.zero;
        private Vector3 _mousePos = Vector3.zero;
        private Rigidbody _rigidbody;
        private float _inputDisableTimer;
        private string[] _outlineColours = new string[] {"Red", "Green", "Blue", "Purple"};

        #region Unity Methods

        private void Awake()
        {
            animator.SetBool("isMoving", false);
            _bow = GetComponentInChildren<Bow>();
            _rigidbody = GetComponent<Rigidbody>();
            _mouseWorld = GetComponent<MouseWorld>();
            _force = GetComponent<Knockback>();

            // state machine test
            StateMachine = new PlayerStateMachine();
            IdleState = new PlayerIdleState(this, StateMachine, "idle");
            MoveState = new PlayerMovingState(this, StateMachine, "move");
            DashState = new PlayerDashingState(this, StateMachine, "dash");
            FallState = new PlayerFallingState(this, StateMachine, "fall");
            StateMachine.Initialize(IdleState);

            grounded = GetComponentInChildren<Grounded>();
        }


        private void Update()
        {
            if (!isLocalPlayer) return;

            _inputDisableTimer -= Time.deltaTime;
            if (_inputDisableTimer < 0)
            {
                _velocity = new Vector3(_movement.x * speed, 0, _movement.y * speed);
                _rigidbody.AddForce(_velocity, ForceMode.Impulse);
                _velocity = Vector3.zero;
            }

            // Max Velocity
            Vector3 horizontalVelocity = _rigidbody.velocity;
            horizontalVelocity.y = 0;
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                _rigidbody.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * _rigidbody.velocity.y;
            }

            // Look Direction (Y)
            facing = Vector3.Normalize(_mouseWorld.Position - transform.position);
            facing.y = 0;
            var rotation = Quaternion.LookRotation(facing);
            var slowRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
            transform.rotation = slowRotation;
            _bow.Dir = facing;

            // dash cd 
            currDashCooldown -= Time.deltaTime;
            if (currDashCooldown < 0)
            {
                currDashCooldown = 0;
            }

            myVelocity = _rigidbody.velocity;

            if (_isBuilding)
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                Build();
            }
            else
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }

        #endregion

        #region Public Methods

        public override void OnStartLocalPlayer()
        {
            // transform.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.layer = LayerMask.NameToLayer(outlineColours[NetworkManager.singleton.numPlayers - 1]);
            if (isLocalPlayer)
            {
                inputs.enabled = true;
            }
        }

        public void DisableInput(float duration)
        {
            _inputDisableTimer = duration;
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

            _movement = context.ReadValue<Vector2>();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (context.started)
            {
                //Debug.Log("Dashing");

                // Use velocity with dash time, don't use add force... this will caus some ramp slides
                if (currDashCooldown <= 0 && IsMoving)
                {
                    Debug.Log("DASH");
                    StartCoroutine(Dash());
                    currDashCooldown += dashCooldown;
                }
                // rigidbody.AddForce((movement * 80), ForceMode.Impulse);
            }
        }


        public void OnShoot(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (context.started)
            {
                Debug.Log("Charging");
                _bow.Charging = true;
            }

            if (context.canceled)
            {
                Debug.Log("Release");
                _bow.shoot(transform.position);
            }
        }

        [Command]
        public void CmdShoot(Vector3 pos, Vector3 dir, Vector3 pow)
        {
            var projectile = Instantiate(arrow);
            projectile.transform.position = _bow.transform.position + dir / 2;
            projectile.GetComponent<Rigidbody>().velocity = pow;
            NetworkServer.Spawn(projectile);
        }
        
        public void OnBuild(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (context.performed)
            {
                _isBuilding = true;
            }

            if (context.canceled)
            {
                _isBuilding = false;
            }
        }
        
        #endregion

        #region Private Methods
        private void Build()
        {
            if (NetworkServer.active)
            {
                if (grounded.active)
                {
                    var position = transform.position;
                    var blockLocation = new Vector3(Mathf.RoundToInt(position.x), 1, Mathf.RoundToInt(position.z));
                    if (!Physics.CheckSphere(blockLocation, 0.1f))
                    {
                        // Check if there's already a block there
                        var newBlock = Instantiate(spawnableBuildBlock, blockLocation, Quaternion.identity);
                        NetworkServer.Spawn(newBlock);
                    }
                }
            }
        }

        private IEnumerator Dash()
        {
            _startingVelocity = new Vector3(_movement.x * speed, 0, _movement.y * speed);
            _startingTime = Time.time;
            var currentY = transform.position.y;
            while (Time.time < _startingTime + dashTimer)
            {
                Transform transform1;
                (transform1 = transform).Translate(_startingVelocity * dashMultiplier * Time.deltaTime, Space.World);
                var position = transform1.position;
                position = new Vector3(position.x, currentY, position.z);
                transform1.position = position;
                yield return null;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Kill"))
            {
                transform.position = NetworkManager.singleton.GetStartPosition().transform.position;
            }
        }

        #endregion
        
    }
}