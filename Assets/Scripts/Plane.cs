using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Plane : MonoBehaviour
{
    private GameInput gameInput;
    private float amountToRollInDegrees = 45;   // Amount of degrees to roll to the left or right when flying left/right
    private float rateOfRoll = 0.25f;   // Amount to roll in a single frame update
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
            transform.Rotate(Vector3.forward, rateOfRoll);
            
            yield return null;
        }
    }

    IEnumerator RollToTheRight()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.Rotate(Vector3.forward, -rateOfRoll);

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
        if(other.tag == Tags.Ramp)
        {
            // Tilt plane back and give it an upward push

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
