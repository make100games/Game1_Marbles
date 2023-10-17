using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    private GameInput gameInput;
    private float lateralDistanceToMove = 1.5f;     // Total distance to move left and right
    private float lateralDistanceToMovePerFrame = 0.01f;    // The amount to move in a single frame when moving left or right

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
    }

    private void MoveRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(MoveRight());
    }

    private void MoveLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(MoveLeft());
    }

    IEnumerator MoveLeft()
    {
        for(float x = 0f; x > -lateralDistanceToMove; x-=lateralDistanceToMovePerFrame)
        {
            Debug.Log("Position.x: " + transform.position.x);
            transform.position =
                new Vector3(transform.position.x - lateralDistanceToMovePerFrame, transform.position.y, transform.position.z);
            yield return null;
        }
    }

    IEnumerator MoveRight()
    {
        for (float x = 0f; x < lateralDistanceToMove; x += lateralDistanceToMovePerFrame)
        {
            Debug.Log("Position.x: " + transform.position.x);
            transform.position =
                new Vector3(transform.position.x + lateralDistanceToMovePerFrame, transform.position.y, transform.position.z);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
