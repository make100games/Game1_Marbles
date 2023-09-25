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
    private int JumpForce = 500;
    private int SpeedIncreaseForce = 10;

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
    }

    void IncreaseSpeed()
    {
        rb.AddForce(Vector3.right * SpeedIncreaseForce, ForceMode.Acceleration);
    }

    public void Jump(int jumpForce, int gravityMultiplier)
    {
        rb.AddForce(Vector3.up * jumpForce * gravityMultiplier, ForceMode.Force);
    }
}

public interface MarbleState
{
    void OnJumpTriggered(int jumpForce, int gravityMultiplier);

    void OnLanded();

    void OnBoosted(int boostForce);
}
