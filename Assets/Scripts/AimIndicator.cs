using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimIndicator : MonoBehaviour
{
    private float value;
    private int lineSegmentCount = 5;
    private List<Vector3> linePoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public float Value { get; set; }

    public void DrawIndicator(Vector3 force, Rigidbody rb, Vector3 startingPoint)
    {
        Vector3 velocity = (force / rb.mass) * Time.fixedDeltaTime;

        float flightDuration = (2 * velocity.y) / Physics.gravity.y;
        float stepTime = flightDuration / lineSegmentCount;

        linePoints.Clear();

        linePoints.Add(startingPoint);

        for (int i = 0; i < lineSegmentCount; i++)
        {
            float stepTimePassed = stepTime * i; //time elapsed
            Vector3 MovementVector = new Vector3(
                velocity.x * stepTimePassed, 
                velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed, 
                velocity.z * stepTimePassed);

            linePoints.Add(-MovementVector*1000 + startingPoint);
        }

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());

    }
    public void Clear()
    {
        linePoints.Clear();
    }
}
