using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWorld : MonoBehaviour
{
    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private float planeHeight;

    public Vector3 Position
    {
        get
        {
            return position;
        }
    }

    // Update is called once per frame
    void Update()
    {

        Plane plane = new Plane(Vector3.up, 0);
        plane.SetNormalAndPosition(Vector3.up, new Vector3(0, planeHeight, 0));

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out distance))
        {
            position = ray.GetPoint(distance);
        }
    }
}