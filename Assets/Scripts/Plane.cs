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
    private float compensatingLateralForce = 10f;   // Force to compensate for movement in opposite direction. ie: If you are currently moving left and then change directions to move right, we want to apply a bit more force while you are still drifing left so that the plane can correct course more quickly
    private float lateralForce = 5f;    // Force to apply when moving left or right
    private float movingSlowlyThreshold = 0.5f; // Speed below which we consider a plane's lateral movement to be slow
    private float decelerationForce = 10f;  // Force at which we decelerate lateral movement when player stops moving laterally
    private float jumpForce = 25f;  // Upward force applied to the plane to make it jump
    private float cruisingYPos; // The y position of the plane when it is just cruising over the surface of the cylinder. This is the y position the plane will come back down to after a jump.
    private float glidingUpwardForce = 20f;  // When we jump, we want to glide back down and not simply fall down. This represents the slight upward force to counteract gravity a bit

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

        cruisingYPos = transform.position.y;
    }

    private void StopMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheLeft());
        movingRight = false;
    }

    private void StartMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheRight());
        movingRight = true;
    }

    private void StopMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheRight());
        movingLeft = false;
    }

    private void StartMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheLeft());
        movingLeft = true;
    }

    IEnumerator RollToTheLeft()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(rateOfRoll, Vector3.forward);
            
            yield return null;
        }
    }

    IEnumerator RollToTheRight()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(-rateOfRoll, Vector3.forward);

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if(movingRight)
        {
            if (rb.velocity.x < 0)
            {
                rb.AddForce(Vector3.right * compensatingLateralForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.right * lateralForce, ForceMode.Acceleration);
        }
        if(movingLeft)
        {
            if(rb.velocity.x > 0)
            {
                rb.AddForce(Vector3.left * compensatingLateralForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.left * lateralForce, ForceMode.Acceleration);
        }
        if(!movingLeft && !movingRight)
        {
            // Decelerate depending if we are currently moving left or right
            if(rb.velocity.x > 0)
            {
                DecelerateRightwardMovement();
            }
            if(rb.velocity.x < 0)
            {
                DecelerateLeftwardMovement();
            }
        }
        if(rb.useGravity && rb.velocity.y < 0)
        {
            // If we are falling after having taken a jump, apply a slight upward force to simulate a gliding effect
            rb.AddForce(Vector3.up * glidingUpwardForce, ForceMode.Force);
        }
    }

    void DecelerateRightwardMovement()
    {
        if (rb.velocity.x < movingSlowlyThreshold)
        {
            // If we are just barely moving, stop the plane's lateral movement
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
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
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
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
        if(other.tag == Tags.Ramp && rb.velocity.y == 0)
        {
            // Tilt plane back then slowly back down and give it an upward push
            // Be sure to enable gravity before applying the jump so that the plane can fall back down
            StartCoroutine(PitchUpQuicklyAndThenSlowlyBackDown());
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.useGravity = true;
        }
    }

    IEnumerator PitchUpQuicklyAndThenSlowlyBackDown()
    {
        // Quickly pitch plane upwards
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfUpwardPitch)
        {
            transform.Rotate(Vector3.left, rateOfUpwardPitch, Space.World);

            yield return null;
        }

        // Slowly pitch it back down
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfDownwardPitch)
        {
            transform.Rotate(Vector3.left, -rateOfDownwardPitch, Space.World);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.velocity.y < 0 && rb.useGravity && rb.position.y < cruisingYPos)
        {
            UnityEngine.Debug.Log("BELOW CRUISING!");
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(new Vector3(rb.position.x, cruisingYPos, rb.position.z));
        }

        // Since the cylinder rotates ever faster, we want to increase the lateral force as well so that the plane can move sideways more quickly as well over time
        compensatingLateralForce += Cylinder.accelerationFactor;
        lateralForce += Cylinder.accelerationFactor;
    }
}
