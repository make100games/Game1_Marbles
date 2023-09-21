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
    public int JumpForce = 500;
    public int SpeedIncreaseForce = 10;

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
        rb.AddForce(Vector3.up * JumpForce * gravityMultiplier, ForceMode.Force);
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
