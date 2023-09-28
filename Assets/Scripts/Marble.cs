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

    private int InitialHorizontalForce = 5;
    private int BoostForce = 10;
    private int HighBoostForce = 50;
    private int JumpForce = 500;
    private int SpeedIncreaseForce = 10;
    private float ContinuousBoost = 0.25f;    // Boost applied while on a continously boosting platform

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

        rb.AddForce(Vector3.right * InitialHorizontalForce, ForceMode.Impulse);
        InvokeRepeating("IncreaseSpeed", 0.5f, 0.5f);
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
            this.CurrentState.OnJumpTriggered(JumpForce, gravityMultiplier);
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

        // We want this to be a regular collider so that a player can land on
        // the continuous boost platform and start boosting
        if(collision.gameObject.tag == Tags.ContinuousBoost)
        {
            if(this.CurrentState != null)
            {
                this.CurrentState.OnStartedContinuousBoost();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tags.Boost)
        {
            if(this.CurrentState != null)
            {
                this.CurrentState.OnBoosted(BoostForce);
            }
        }
        if (other.gameObject.tag == Tags.HighBoost)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.OnBoosted(HighBoostForce);
            }
        }
        if (other.gameObject.tag == Tags.ContinuousBoostExit)
        {
            if (this.CurrentState != null)
            {
                this.CurrentState.OnStoppedContinuousBoost();
            }
        }
    }

    void IncreaseSpeed()
    {
        rb.AddForce(Vector3.right * SpeedIncreaseForce, ForceMode.Acceleration);
    }

    public void Jump(int jumpForce, int gravityMultiplier)
    {
        rb.AddForce(Vector3.up * jumpForce * gravityMultiplier, ForceMode.Force);
    }

    public void ApplySoftBoost()
    {
        rb.AddForce(Vector3.right * ContinuousBoost, ForceMode.Impulse);
    }
}

public interface MarbleState
{
    void OnJumpTriggered(int jumpForce, int gravityMultiplier);

    void OnLanded();

    void OnBoosted(int boostForce);

    void OnStartedContinuousBoost();

    void OnStoppedContinuousBoost();
}
