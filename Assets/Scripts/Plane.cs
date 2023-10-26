using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Plane : MonoBehaviour
{
    private GameInput gameInput;
    private float amountToRollInDegrees = 45;   // Amount of degrees to roll to the left or right when flying left/right
    private float rateOfRoll = 0.25f;   // Amount to roll in a single frame update
    private float amountToPitchInDegrees = 45;  // Amount of degrees to pitch plane back when we hit a ramp
    private float rateOfUpwardPitch = 0.25f;    // Amount to pitch in a single frame update when pitching up
    private float rateOfDownwardPitch = 0.0725f;   // Amount to pitch in a single frame update when pitching down
    private Rigidbody rb;
    private bool movingLeft = false;
    private bool movingRight = false;
    private float movingSlowlyThreshold = 0.5f; // Speed below which we consider a plane's lateral movement to be slow
    private float decelerationForce = 10f;  // Force at which we decelerate lateral movement when player stops moving laterally

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        rb = GetComponent<Rigidbody>();

        gameInput.Game.StartMovingLeft.performed += StartMovingLeft_performed;
        gameInput.Game.StopMovingLeft.performed += StopMovingLeft_performed;
        gameInput.Game.StartMovingRight.performed += StartMovingRight_performed;
        gameInput.Game.StopMovingRight.performed += StopMovingRight_performed;
    }

    private void StopMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UnityEngine.Debug.Log("Stop moving right");
        StartCoroutine(RollToTheLeft());
        movingRight = false;
    }

    private void StartMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UnityEngine.Debug.Log("Start moving right");
        StartCoroutine(RollToTheRight());
        movingRight = true;
    }

    private void StopMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UnityEngine.Debug.Log("Stop moving left");
        StartCoroutine(RollToTheRight());
        movingLeft = false;
    }

    private void StartMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UnityEngine.Debug.Log("Start moving left");
        StartCoroutine(RollToTheLeft());
        movingLeft = true;
    }

    IEnumerator RollToTheLeft()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(rateOfRoll, Vector3.forward);
            //transform.Rotate(Vector3.forward, rateOfRoll);
            
            yield return null;
        }
    }

    IEnumerator RollToTheRight()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(-rateOfRoll, Vector3.forward);
            //transform.Rotate(Vector3.forward, -rateOfRoll);

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if(movingRight)
        {
            if (rb.velocity.x < 0)
            {
                rb.AddForce(Vector3.right * 10f, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.right * 5f, ForceMode.Acceleration);
        }
        if(movingLeft)
        {
            if(rb.velocity.x > 0)
            {
                rb.AddForce(Vector3.left * 10f, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.left * 5f, ForceMode.Acceleration);
        }
        if(!movingLeft && !movingRight)
        {
            // Decelerate depending if we are currently moving left or right
            //UnityEngine.Debug.Log("Rigid body velocity: " + rb.velocity);
            if(rb.velocity.x > 0)
            {
                DecelerateRightwardMovement();
            }
            if(rb.velocity.x < 0)
            {
                DecelerateLeftwardMovement();
            }
        }
    }

    void DecelerateRightwardMovement()
    {
        if (rb.velocity.x < movingSlowlyThreshold)
        {
            // If we are just barely moving, stop the plane's lateral movement
            UnityEngine.Debug.Log("STOP");
            rb.velocity = Vector3.zero;
        }
        else
        {
            // We are moving to the right so decelerate by applying force to the left
            rb.AddForce(Vector3.left * decelerationForce, ForceMode.Acceleration);
        }
    }

    void DecelerateLeftwardMovement()
    {
        if (rb.velocity.x > -movingSlowlyThreshold)
        {
            // If we are just barely moving, stop the plane's lateral movement
            UnityEngine.Debug.Log("STOP");
            rb.velocity = Vector3.zero;
        }
        else
        {
            // We are moving to the right so decelerate by applying force to the left
            rb.AddForce(Vector3.right * decelerationForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.Debug.Log("Triggery trigger: " + other.tag);
        if(other.tag == Tags.Ramp)
        {
            // Tilt plane back then slowly back down and give it an upward push
            StartCoroutine(PitchUpQuicklyAndThenSlowlyBackDown());
        }
    }

    IEnumerator PitchUpQuicklyAndThenSlowlyBackDown()
    {
        // Quickly pitch plane upwards
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfUpwardPitch)
        {
            //transform.rotation *= Quaternion.AngleAxis(rateOfUpwardPitch, Vector3.left);
            transform.Rotate(Vector3.left, rateOfUpwardPitch, Space.World);

            yield return null;
        }

        // Slowly pitch it back down
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfDownwardPitch)
        {
            //transform.rotation *= Quaternion.AngleAxis(-rateOfDownwardPitch, Vector3.left);
            transform.Rotate(Vector3.left, -rateOfDownwardPitch, Space.World);

            yield return null;
        }
    }

    private float duration = 150f;
    private float timeRemaining = 150f;

    // Update is called once per frame
    void Update()
    {
        var targetDirection = Quaternion.Euler(Vector3.forward * 45);
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetDirection, duration - timeRemaining);
        //timeRemaining -= Time.deltaTime;

        //transform.Rotate(Vector3.right, 50.0f * Time.deltaTime, Space.World);
    }
}
