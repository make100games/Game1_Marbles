using UnityEngine;
using System.Collections;

public class RotatingDisc : MonoBehaviour
{
    public float rotationSpeed = 30f;

    void Start()
    {
        StartCoroutine(RotateObject());
    }

    IEnumerator RotateObject()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
