using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private float mass = 1; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private CharacterController character;
 
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += -dir.normalized * force / mass;
        Debug.Log(impact.magnitude);
    }

    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2)
        {
            Debug.Log("moving player");
            character.Move(impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}
