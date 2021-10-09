using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Experimental;
using RDPolarity.Arena;
using RDPolarity.Player;
using RDPolarity.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
        [SerializeField] private Vector3 facing = Vector3.zero;
        [SerializeField] private GameObject spawnableBuildBlock;
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject swordSlash;
        [SerializeField] private GameObject onDeathParticles;
        [SerializeField] private GameObject onHitParticles;
        [SerializeField] public string currState = "";
        private PlayerInfo playerInfo;
        [SerializeField] private bool disabled;

        [SyncVar(hook = nameof(OnStocksChange))] private int _stocks = 3;

        // ReadOnly Debug
        [Title("Debug Variables")] [SerializeField, ReadOnly]
        private bool isMoving = false;
        [SerializeField, ReadOnly] private bool isDashing = false;
        [SerializeField, ReadOnly] private bool isGrounded = false;

        [SerializeField, ReadOnly] private bool isBuilding;

        // Events
        [Serializable]
        public class OnDeathEvent : UnityEvent
        {
        }

        public OnDeathEvent onDeathEvent = new OnDeathEvent();

        public delegate void OnLose(PlayerController p);
        public static event OnLose ONLoseEvent;

        public delegate void OnConnect(PlayerController p);
        public static event OnConnect ONConnectEvent;

        public class OnChargeEvent : UnityEvent
        {
        }

        private readonly OnChargeEvent _onChargeEvent = new OnChargeEvent();

        [Serializable]
        public class OnFireEvent : UnityEvent
        {
        }

        public OnFireEvent onFireEvent = new OnFireEvent();

        [Serializable]
        public class OnDashEvent : UnityEvent
        {
        }

        public OnDashEvent onDashEvent = new OnDashEvent();

        [Serializable]
        public class OnStrikeEvent : UnityEvent
        {
        }

        public OnStrikeEvent onStrikeEvent = new OnStrikeEvent();

        [Serializable]
        public class OnBuildEvent : UnityEvent
        {
        }

        public OnBuildEvent onBuildEvent = new OnBuildEvent();

        [SerializeField] private GameObject modelSwapper;
        [SerializeField] private List<GameObject> skinList = new List<GameObject>();
        private int selectedSkin = 0;
        
        // Properties
        public bool IsMoving => isMoving;
        public bool checkMoving() { return isMoving; }
        public bool checkDashing() { return isDashing; }
        public bool checkGrounded() { return isGrounded; }

        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMovingState MoveState { get; private set; }
        public PlayerDashingState DashState { get; private set; }
        public PlayerFallingState FallState { get; private set; }
        

        [Serializable]
        public class OnHitEvent : UnityEvent
        {
        }

        public OnHitEvent onHitEvent = new OnHitEvent();

        [Serializable]
        public class OnHitSelfEvent : UnityEvent
        {
        }

        public OnHitSelfEvent onHitSelfEvent = new OnHitSelfEvent();

        [Serializable]
        public class OnHitOthersEvent : UnityEvent
        {
        }

        public OnHitOthersEvent onHitOthersEvent = new OnHitOthersEvent();

        // Local Variables
        private MouseWorld _mouseWorld;
        private Knockback _force;
        private float _startingTime;

        private Vector3 _velocity = Vector3.zero;
        private Vector3 _startingVelocity = Vector3.zero;
        private Bow _bow;
        private Vector2 _movement = Vector2.zero;
        private Vector3 _mousePos = Vector3.zero;
        private Rigidbody _rigidbody;
        private float _inputDisableTimer;
        private string[] _outlineColours = new string[] {"Red", "Green", "Blue", "Purple"};
        private bool _isMaxVelocity = true;
        private Quaternion slowRotation;

        public void OnSkinForward(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (selectedSkin >= skinList.Count - 1) { selectedSkin = 0; } 
                else { selectedSkin++; }
                
                foreach (Transform child in modelSwapper.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                var skin = Instantiate(skinList[selectedSkin], modelSwapper.transform);
                animator = skin.GetComponent<Animator>();
                StateMachine.CurrentState.SetAnim(animator);
                animator.SetBool("isMoving", false);
            }
            
        }

        public void OnSkinBackward(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (selectedSkin <= 0) { selectedSkin = skinList.Count - 1; }
                else { selectedSkin--; }
                
                foreach (Transform child in modelSwapper.transform) {
                    GameObject.Destroy(child.gameObject);
                }
                var skin = Instantiate(skinList[selectedSkin], modelSwapper.transform);
                animator = skin.GetComponent<Animator>();
                StateMachine.CurrentState.SetAnim(animator);
                animator.SetBool("isMoving", false);
            }
        }
        
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
            StateMachine.CurrentState.SetAnim(animator);

            grounded = GetComponentInChildren<Grounded>();
        }

        private void Start()
        {
            if (!isServer) return;
            MatchManager.DisablePlayers += ChangePlayerDisable;
            ONConnectEvent?.Invoke(this);
        }

        private void ChangePlayerDisable(bool value)
        {
            Debug.Log("Player Disable Changed To " + value.ToString());
            disabled = value;
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            if (disabled) return;

            _inputDisableTimer -= Time.deltaTime;
            if (_inputDisableTimer < 0)
            {
                _velocity = new Vector3(_movement.x * speed, 0, _movement.y * speed);
                _rigidbody.AddForce(_velocity, ForceMode.Impulse);
                _velocity = Vector3.zero;
            }

            // Max Velocity
            if (_isMaxVelocity)
            {
                Vector3 horizontalVelocity = _rigidbody.velocity;
                horizontalVelocity.y = 0;
                if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
                {
                    _rigidbody.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * _rigidbody.velocity.y;
                }
            }

            // Look Direction (Y)
            transform.rotation = slowRotation;

            // dash cd 
            currDashCooldown -= Time.deltaTime;
            if (currDashCooldown < 0)
            {
                currDashCooldown = 0;
            }


            isGrounded = grounded.active;
            myVelocity = _rigidbody.velocity;
            StateMachine.stateUpdate();
            currState = StateMachine.CurrentState.CheckState();

            if (isBuilding)
            {
                // _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
                Build();
            }
            else
            {
                // _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
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

        private bool _canStrike = true;

        public void OnStrike(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;

            if (context.performed)
            {
                if (_canStrike)
                {
                    onStrikeEvent.Invoke();
                    CmdStrike(gameObject);
                    _canStrike = false;
                    StartCoroutine(StartStrikeCooldown());
                }
            }
        }

        [Command]
        public void CmdStrike(GameObject player)
        {
            var effect = Instantiate(swordSlash, player.transform.position, player.transform.rotation);
            effect.AddComponent<StickToTransform>().target = player.transform;
            NetworkServer.Spawn(effect);
        }

        IEnumerator StartStrikeCooldown()
        {
            yield return new WaitForSeconds(0.5f);
            _canStrike = true;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            Debug.Log("player moving");
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

        public void OnLook(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;

            if (context.control.displayName == "Right Stick")
            {
                Vector2 stickInput = Gamepad.current.rightStick.ReadValue().normalized;

                //Dont update the facing if the input is too small. Player can shoot downwards if its allowed
                if (Mathf.Abs(stickInput.x + stickInput.y) > 0.02)
                {
                    facing = new Vector3(stickInput.x, 0, stickInput.y);
                }
            }

            else
            {
                facing = Vector3.Normalize(_mouseWorld.Position - transform.position);
                facing.y = 0;
                
            }
            var rotation = Quaternion.LookRotation(facing);
            slowRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
            
            _bow.Dir = facing;
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
                    onDashEvent.Invoke();
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
                Debug.Log(context.control.displayName);
                _bow.Charging = true;
                if (_bow.CooldownTimer > 0) _onChargeEvent.Invoke();
            }

            if (context.canceled)
            {
                Debug.Log("Release");
                Debug.Log(context.control.displayName);
                onFireEvent.Invoke();
                _bow.Shoot(transform.position);
            }
        }

        public bool Alive()
        {
            if (_stocks > 0)
            {
                return true;
            }
            return false;
        }

        public void OnBuild(InputAction.CallbackContext context)
        {
            if (!isLocalPlayer) return;
            if (context.performed)
            {
                isBuilding = true;
            }

            if (context.canceled)
            {
                isBuilding = false;
            }
        }

        public void SetPlayerInfo(PlayerInfo _playerInfo)
        {
            playerInfo = _playerInfo;
        }

        #endregion

        #region Private Methods

        private void Build()
        {
            if (grounded.active)
            {
                var position = transform.position;
                var blockLocation = new Vector3(Mathf.RoundToInt(position.x), 1, Mathf.RoundToInt(position.z));
                if (!Physics.CheckSphere(blockLocation, 0.1f)) // Check if there's already a block there
                {
                    CmdPlaceBlock(blockLocation);
                    onBuildEvent.Invoke();
                }
            }
        }

        private void OnStocksChange(int oldValue, int newValue)
        {
            playerInfo.UpdateStock(_stocks);
        }

        [Command]
        private void CmdPlaceBlock(Vector3 blockLocation)
        {
            var newBlock = Instantiate(spawnableBuildBlock, blockLocation, Quaternion.identity);
            NetworkServer.Spawn(newBlock);
        }

        private IEnumerator Dash()
        {
            _startingVelocity = new Vector3(_movement.x * speed, 0, _movement.y * speed);
            _startingTime = Time.time;
            var currentY = transform.position.y;
            isDashing = true;

            var oldSpeed = speed;
            speed *= dashMultiplier;
            while (Time.time < _startingTime + dashTimer)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized;
                yield return null;
            }
            speed = oldSpeed;
            isDashing = false;
        }

        private void UpdateStocks()
        {
            _stocks--;
            if (_stocks <= 0)
            {
                ONLoseEvent.Invoke(this);
                if (!Alive())
                {
                    ChangePlayerDisable(true);
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Kill"))
            {
                onDeathEvent.Invoke();
                UpdateStocks();
                Instantiate(onDeathParticles, transform.position, transform.rotation);
                transform.position = NetworkManager.singleton.GetStartPosition().transform.position;
            }
            
            if (collision.gameObject.CompareTag("Arrow"))
            {
                onHitEvent.Invoke();
                if (!isLocalPlayer) onHitOthersEvent.Invoke();
                var arrowVel = collision.GetComponentInParent<Rigidbody>().velocity;
                Instantiate(onHitParticles, transform.position, transform.rotation);
                StartCoroutine(MaxVelocityCooldown());
                _rigidbody.AddForce(arrowVel * 2, ForceMode.Impulse);
                
                var collisionArrow = collision.transform.parent;
                Destroy(collisionArrow.gameObject);
            }
        }

        IEnumerator MaxVelocityCooldown()
        {
            _isMaxVelocity = false;
            yield return new WaitForSeconds(0.1f);
            _isMaxVelocity = true;
        }

        #endregion
    }
}