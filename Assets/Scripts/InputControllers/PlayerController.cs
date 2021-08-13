using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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

    private Rigidbody rigidbody;

    private Vector2 movement = Vector2.zero;

    private Vector3 mousePos = Vector3.zero;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
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
        var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out var hit);
        var lookPos = hit.point - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
            animator.SetBool("isMoving", true);
        if (context.canceled)
            animator.SetBool("isMoving", false);
        movement = context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("SHOT!");
        }
    }
}
