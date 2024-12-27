using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The StartScreenCam oscillates from side to side to create a cool looking weave effect
/// </summary>
public class StartScreenCam : MonoBehaviour
{
    public float amplitude = 1f; // The maximum distance from the starting point
    public float frequency = 1f; // The speed of the oscillation
    public Vector3 axis = Vector3.right; // The axis of movement (e.g., side-to-side)

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the sine wave movement
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the offset along the specified axis
        transform.position = startPosition + axis * offset;
    }
}
