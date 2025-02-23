using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder : MonoBehaviour
{
    public const float accelerationFactor = 0.00025f;   // Amount by which to increase rotation speed of cylinder
    private const float deccelerationFactorWhenPlaneCrashed = 0.0075f; // Amount by which to decrease rotation once the player has crashed
    private bool boostActive = false;
    private float boostFactor = 50f; // Amount by which to increase speed when boosting
    private float RotationSpeed { get; set; } = -50f;   // Originally at -40 The more negative the value, the faster the cylinder rotates
    private bool isAccelerating = false;

    public float RotationSpeedIgnoringBoost
    {
        get
        {
            if (boostActive)
            {
                return RotationSpeed + boostFactor;
            }
            else
            {
                return RotationSpeed;
            }
        }
    }

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
        else if(isAccelerating)
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

    public void StartBoost()
    {
        this.boostActive = true;
        this.RotationSpeed -= boostFactor;
    }

    public void StopBoost()
    {
        this.boostActive = false;
        this.RotationSpeed += boostFactor;
    }

    public void StartAccelerating()
    {
        this.isAccelerating = true;
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
    }
}
