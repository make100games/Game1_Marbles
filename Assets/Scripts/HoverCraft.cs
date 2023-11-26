using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraft : MonoBehaviour
{
    public bool acceleratesOverTime = false;
    private const float accelerationFactor = 0.0005f;   // Amount by which to increase speed of craft
    public float rotationSpeed = 15f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(acceleratesOverTime)
        {
            rotationSpeed += accelerationFactor;
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
