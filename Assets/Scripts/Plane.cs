using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    private GameInput gameInput;
    private float amountToRollInDegrees = 45;   // Amount of degrees to roll to the left or right when flying left/right
    private float rateOfRoll = 0.25f;   // Amount to roll in a single frame update

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();

        gameInput.Game.StartMovingLeft.performed += StartMovingLeft_performed;
        gameInput.Game.StopMovingLeft.performed += StopMovingLeft_performed;
        gameInput.Game.StartMovingRight.performed += StartMovingRight_performed;
        gameInput.Game.StopMovingRight.performed += StopMovingRight_performed;
    }

    private void StopMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Stop moving right");
        StartCoroutine(RollToTheLeft());
    }

    private void StartMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Start moving right");
        StartCoroutine(RollToTheRight());
    }

    private void StopMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Stop moving left");
        StartCoroutine(RollToTheRight());
    }

    private void StartMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("Start moving left");
        StartCoroutine(RollToTheLeft());
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
