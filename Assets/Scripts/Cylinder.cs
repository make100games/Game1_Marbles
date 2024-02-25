using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder : MonoBehaviour
{
    public const float accelerationFactor = 0.00025f;   // Amount by which to increase rotation speed of cylinder
    private const float deccelerationFactorWhenPlaneCrashed = 0.0075f; // Amount by which to decrease rotation once the player has crashed
    public float RotationSpeed { get; private set; } = -40f;
    private bool comeToAStop = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(comeToAStop)
        {
            if(RotationSpeed < 0)
            {
                RotationSpeed += deccelerationFactorWhenPlaneCrashed;
            }
            else if(RotationSpeed > 0)
            {
                RotationSpeed = 0;
            }
        }
        else
        {
            // The player speed is nothing more than the ever increasing speed of
            // rotation of the cylinder. Increasing in the negative direction ;)
            RotationSpeed -= accelerationFactor;
        }
        
    }

    public void ComeToAStop()
    {
        comeToAStop = true;
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
    }
}
