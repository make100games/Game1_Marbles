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

    public int InitialHorizontalForce = 100;
    public int BoostForce = 10;
    public int JumpForce = 500;
    public int SpeedIncreaseForce = 10;

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
        Debug.Log("Fall Down");
        gravityMultiplier = 1;
        Physics.gravity = gravity;
    }

    private void FallUp_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("Fall Up");
        gravityMultiplier = -1;
        Physics.gravity *= gravityMultiplier;
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        Debug.Log("On Jump");
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
            rb.AddForce(Vector3.right * 10, ForceMode.Impulse);
        }
    }

    void IncreaseSpeed()
    {
        rb.AddForce(Vector3.right * SpeedIncreaseForce, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
    }

    
}

public interface MarbleState
{
    void OnJumpTriggered(int jumpForce, int gravityMultiplier);

    void OnLanded();
}
