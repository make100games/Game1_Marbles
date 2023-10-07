using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Marble : MonoBehaviour
{
    private Rigidbody rb;
    private GameInput gameInput;
    private float timePassed = 0f;
    private Vector3 gravity;
    private int gravityMultiplier = 1;  // Toggles between 1 and -1 depending on if we are falling down or up

    private int initialHorizontalForce = 5;
    private int boostForce = 10;
    private int highBoostForce = 50;
    private int jumpForce = 500;
    private float acceleratingBoost = 10f;   // Continuously apply boost while on the ground
    private float slamDownForce = 500;  // While in the air, the player can slam the ball back to the ground
    private float landingBoost = 10;    // The boost you get when you land right after an obstacle
    private bool isAccelerating = false;

    public MarbleState CurrentState { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        rb = GetComponent<Rigidbody>();

        gameInput.Game.Jump.performed += Jump_performed;
        gameInput.Game.FallUp.performed += FallUp_performed;
        gameInput.Game.FallDown.performed += FallDown_performed;

        gravity = Physics.gravity;

        rb.AddForce(Vector3.right * initialHorizontalForce, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if(isAccelerating)
        {
            rb.AddForce(Vector3.right * acceleratingBoost, ForceMode.Acceleration);
        }
    }

    private void FallDown_performed(InputAction.CallbackContext obj)
    {
        gravityMultiplier = 1;
        Physics.gravity = gravity;
    }

    private void FallUp_performed(InputAction.CallbackContext obj)
    {
        gravityMultiplier = -1;
        Physics.gravity *= gravityMultiplier;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        if(this.CurrentState != null)
        {
            this.CurrentState.OnJumpTriggered(jumpForce, gravityMultiplier);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == Tags.Ground)
        {
            if(this.CurrentState != null)
            {
                this.CurrentState.OnLanded();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tags.Boost)
        {
            if(this.CurrentState != null)
            {
                this.CurrentState.OnBoosted(boostForce);
            }
        }
        if (other.gameObject.tag == Tags.HighBoost)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.OnBoosted(highBoostForce);
            }
        }
    }

    public void Jump(int jumpForce, int gravityMultiplier)
    {
        rb.AddForce(Vector3.up * jumpForce * gravityMultiplier, ForceMode.Force);
    }

    public void SlamDown()
    {
        rb.AddForce(Vector3.down * slamDownForce, ForceMode.Force);
    }

    public void StartAccelerating()
    {
        isAccelerating = true;
    }

    public void StopAccelerating()
    {
        isAccelerating = false;
    }

    public void ApplyLandingBoost()
    {
        rb.AddForce(Vector3.right * landingBoost, ForceMode.Impulse);
    }
}

public interface MarbleState
{
    void OnJumpTriggered(int jumpForce, int gravityMultiplier);

    void OnLanded();

    void OnBoosted(int boostForce);
}
