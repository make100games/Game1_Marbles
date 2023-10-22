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
            UnityEngine.Debug.Log("Move right");
            rb.AddForce(Vector3.right * 5f, ForceMode.Acceleration);
        }
        if(movingLeft)
        {
            UnityEngine.Debug.Log("Move left");
            rb.AddForce(Vector3.left * 5f, ForceMode.Acceleration);
        }
        if(!movingLeft && !movingRight)
        {
            // Decelerate depending if we are currently moving left or right
            UnityEngine.Debug.Log("Rigid body velocity: " + rb.velocity);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
