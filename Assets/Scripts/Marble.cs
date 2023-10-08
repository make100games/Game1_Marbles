using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private GameInput gameInput;

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();

        gameInput.Game.Jump.performed += Jump_performed;
        gameInput.Game.MoveLeft.performed += MoveLeft_performed;
        gameInput.Game.MoveRight.performed += MoveRight_performed;
    }

    private void MoveRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
    }

    private void MoveLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
