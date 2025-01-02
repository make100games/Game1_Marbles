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
        var amountToRotatePerFrame = -0.075f;
        while(amountRotated > -85f)
        {
            gameTitleCanvas.transform.Rotate(Vector3.left * amountToRotatePerFrame, Space.Self);
            amountRotated += amountToRotatePerFrame;
            yield return null;
        }
        foldedDown = true;
    }

    private IEnumerator MoveOutOfSight()
    {
        var amountRotated = 0f;
        var amountToRotatePerFrame = 0.025f;
        while (amountRotated < 45f)
        {
            transform.Rotate(Vector3.up * amountToRotatePerFrame, Space.Self);
            amountRotated += amountToRotatePerFrame;
            yield return null;
        }
        movedOutOfWay = true;
    }
}
