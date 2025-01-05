using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTitle : MonoBehaviour
{
    private bool foldedDown = false;
    private bool movedOutOfWay = false;
    public GameObject gameTitleCanvas;

    public event Action OnTitleDismissed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(foldedDown && movedOutOfWay)
        {
            // make sure we only invoke the callback once
            foldedDown = false;
            movedOutOfWay = false;

            OnTitleDismissed?.Invoke();
        }
    }

    /// <summary>
    /// Folds title downwards and moves it out of view past the camera
    /// </summary>
    public void Dismiss()
    {
        StartCoroutine(FoldDown());
        StartCoroutine(MoveOutOfSight());
    }

    private IEnumerator FoldDown()
    {
        var amountRotated = 0f;
        var rotationSpeed = -75f; // Rotation speed in degrees per second
        while (amountRotated > -85f)
        {
            // Calculate rotation for this frame
            var rotationThisFrame = rotationSpeed * Time.deltaTime;
            gameTitleCanvas.transform.Rotate(Vector3.left * rotationThisFrame, Space.Self);
            amountRotated += rotationThisFrame;
            yield return null;
        }
        foldedDown = true;
    }

    private IEnumerator MoveOutOfSight()
    {
        var amountRotated = 0f;
        var rotationSpeed = 25f; // Rotation speed in degrees per second
        while (amountRotated < 45f)
        {
            // Calculate rotation for this frame
            var rotationThisFrame = rotationSpeed * Time.deltaTime;

            // Rotate the transform and update the total rotated amount
            transform.Rotate(Vector3.up * rotationThisFrame, Space.Self);
            amountRotated += rotationThisFrame;

            yield return null;
        }
        movedOutOfWay = true;
    }
}
