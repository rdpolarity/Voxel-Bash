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

    public void DrawIndicator(Vector3 force, Vector3 startingPoint)
    {
        linePoints.Clear();

        //This is a rough estimate. gets inaccurate near the end of it, but for now it does actually point to the right block up to about a 8 block radius.
        //Tried doing legitimate trajectory but it wasn't having it
        linePoints.Add(startingPoint);
        var distanceMultiplier = Vector3.Distance(startingPoint, startingPoint + force)*0.05f;
        linePoints.Add((startingPoint -force.normalized) + force / 2 + Vector3.down*distanceMultiplier);

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());

    }
    public void Clear()
    {
        linePoints.Clear();
    }
}
