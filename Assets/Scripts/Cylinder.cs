using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder : MonoBehaviour
{
    public const float accelerationFactor = 0.00025f;   // Amount by which to increase rotation speed of cylinder
    private float rotationSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotationSpeed += accelerationFactor;
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
