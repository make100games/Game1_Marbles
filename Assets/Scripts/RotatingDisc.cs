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
        while (rotationSpeed > 0)
        {
            rotationSpeed -= 0.00025f;
            yield return null;
        }
        if(rotationSpeed <= 0 && !onStoppedInvoked)
        {
            onStoppedInvoked = true;
            OnStoppedRotating?.Invoke();
        }
    }
}
