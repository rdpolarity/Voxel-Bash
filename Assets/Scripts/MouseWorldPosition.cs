using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWorldPosition : MonoBehaviour
{
    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private float planeHeight;
    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 Position { get; set; }

    // Update is called once per frame
    void Update()
    {

        Plane plane = new Plane(Vector3.up, 0);
        plane.SetNormalAndPosition(Vector3.up, new Vector3(0, planeHeight, 0));
        DrawPlane(Vector3.up, plane.normal);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (plane.Raycast(ray, out distance))
        {
            position = ray.GetPoint(distance);
        }
    }

    void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
        {
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        }
        else
        {
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
        }
    }
}
