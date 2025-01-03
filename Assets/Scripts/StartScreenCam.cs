using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;
using System;

/// <summary>
/// The StartScreenCam oscillates from side to side to create a cool looking weave effect
/// </summary>
public class StartScreenCam : MonoBehaviour
{
    public float amplitude = 1f; // The maximum distance from the starting point
    public float frequency = 1f; // The speed of the oscillation
    public Vector3 axis = Vector3.right; // The axis of movement (e.g., side-to-side)
    public GameObject gameTitle;
    public GameObject gameCam;

    public event Action OnTitleFullyDisplayed;
    public event Action OnTitleDismissed;

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
        gameTitle.GetComponent<GameTitle>().OnTitleDismissed += StartScreenCam_OnTitleDismissed;
        gameTitle.GetComponent<RotatingDisc>().OnStoppedRotating += StartScreenCam_OnStoppedRotating;
    }

    private void StartScreenCam_OnStoppedRotating()
    {
        OnTitleFullyDisplayed?.Invoke();
    }

    private void StartScreenCam_OnTitleDismissed()
    {
        this.OnTitleDismissed?.Invoke();
    }

    void Update()
    {
        // Shoot a ray to detect whether camera sees the game title. Once camera
        // sees game title, stop game title from getting any closer to camera
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        var rayDistance = 100f;

        Debug.DrawRay(origin, direction * rayDistance, Color.red, 2f);

        // Perform the raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            // Log the name of the object hit
            if(hit.collider.gameObject.tag == Tags.GameTitle)
            {
                gameTitle.GetComponent<RotatingDisc>().StopRotating();
            }
        }
    }

    private void FixedUpdate()
    {
        // Calculate the sine wave movement
        float offset = Mathf.Sin(Time.time * frequency) * amplitude;

        // Apply the offset along the specified axis
        transform.position = startPosition + axis * offset;
    }

    public void StartPlaying()
    {
        gameTitle.GetComponent<GameTitle>().Dismiss();
    }
}
