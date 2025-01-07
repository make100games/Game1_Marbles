using UnityEngine;
using System.Collections;
using System;

public class RotatingDisc : MonoBehaviour
{
    public float rotationSpeed = 30f;
    private bool onStoppedInvoked = false;

    public event Action OnStoppedRotating;

    void Start()
    {
        StartCoroutine(RotateObject());
    }

    IEnumerator RotateObject()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void StopRotating()
    {
        StartCoroutine(SlowDownTillStopped());
    }

    private IEnumerator SlowDownTillStopped()
    {
        float decelerationRate = 0.25f; // Deceleration rate in units per second

        while (rotationSpeed > 0)
        {
            // Reduce rotation speed based on the deceleration rate and deltaTime
            rotationSpeed -= decelerationRate * Time.deltaTime;

            // Clamp rotationSpeed to ensure it doesn't go below zero
            rotationSpeed = Mathf.Max(rotationSpeed, 0);

            yield return null;
        }

        if (rotationSpeed <= 0 && !onStoppedInvoked)
        {
            onStoppedInvoked = true;
            OnStoppedRotating?.Invoke();
        }
    }
}
