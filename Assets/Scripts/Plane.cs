using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Plane : MonoBehaviour
{
    public Transform shipSparksTransform;   // Used by the electric sparks particle system so that it follows the ship but does not rotate with the ship. Can be just some transform in the scene (e.g. an empty GameObject)
    public GameObject cylinder; // The cylinder around which the plane is flying. Technically the plane is standing still and the cylinder is spinning but you know what I mean
    public GameObject boundaryCollisionSparks;  // The sparks which appear as a result of the ship hitting the left or right stage boundary
    public GameObject lightSmoke;   // The smoke to show when plane has been hit once
    public GameObject moreSmoke;    // Smoke to show when plane has been hit twice
    public GameObject sparks1;   // One of the spark particle systems to play back when plane has been hit twice
    public GameObject sparks2;   // Another of the spark particle systems to play back when plane has been hit twice
    public GameObject sparks3;   // Another of the spark particle systems to play back when plane has been hit twice
    public GameObject sparks4;   // Another of the spark particle systems to play back when plane has been hit twice
    public GameObject trackingCamera;   // The camera tracking the plane
    public Volume blurVolume;
    private Cylinder cylinderScript;
    private CinemachineBasicMultiChannelPerlin cameraShaker;    // The part of the camera that controls how the camera shakes
    private GameInput gameInput;
    private float amountToRollInDegrees = 45;   // Amount of degrees to roll to the left or right when flying left/right
    private float rateOfRoll = 0.25f;   // Amount to roll in a single frame update
    private float amountToPitchInDegrees = 45;  // Amount of degrees to pitch plane back when we hit a ramp
    private float rateOfUpwardPitch = 0.25f;    // Amount to pitch in a single frame update when pitching up
    private float rateOfDownwardPitch = 0.0725f;   // Amount to pitch in a single frame update when pitching down
    private Rigidbody rb;
    private bool movingLeft = false;
    private bool movingRight = false;
    private float compensatingLateralForce = 25f;   // Force to compensate for movement in opposite direction. ie: If you are currently moving left and then change directions to move right, we want to apply a bit more force while you are still drifing left so that the plane can correct course more quickly
    private float lateralForce = 20f;    // Force to apply when moving left or right
    private float movingSlowlyThreshold = 0.5f; // Speed below which we consider a plane's lateral movement to be slow
    private float decelerationForce = 10f;  // Force at which we decelerate lateral movement when player stops moving laterally
    private float jumpForce = 25f;  // Upward force applied to the plane to make it jump
    private float cruisingYPos; // The y position of the plane when it is just cruising over the surface of the cylinder. This is the y position the plane will come back down to after a jump.
    private float crashedYPos;  // The y position of the plane when it has hit the ground after a crash (game over)
    private float glidingUpwardForce = 20f;  // When we jump, we want to glide back down and not simply fall down. This represents the slight upward force to counteract gravity a bit
    private int numberOfCoinsCollected = 0;
    private int health = 3; // Plane can take 3 hits before crashing
    private bool dead = false;  // True if the player has crashed the plane
    private CinemachineCollisionImpulseSource collisionImpulseSource;
    private DepthOfField dizzinessBlur;

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        rb = GetComponent<Rigidbody>();
        cylinderScript = cylinder.GetComponent<Cylinder>();
        cameraShaker = trackingCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        collisionImpulseSource = GetComponent<CinemachineCollisionImpulseSource>();
        DepthOfField temp;
        if(blurVolume.profile.TryGet<DepthOfField>(out temp))
        {
            dizzinessBlur = temp;
        }

        gameInput.Game.StartMovingLeft.performed += StartMovingLeft_performed;
        gameInput.Game.StopMovingLeft.performed += StopMovingLeft_performed;
        gameInput.Game.StartMovingRight.performed += StartMovingRight_performed;
        gameInput.Game.StopMovingRight.performed += StopMovingRight_performed;

        cruisingYPos = transform.position.y;
        crashedYPos = cruisingYPos - 3;
    }

    private void StopMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheLeft());
        movingRight = false;
    }

    private void StartMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheRight());
        movingRight = true;
    }

    private void StopMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheRight());
        movingLeft = false;
    }

    private void StartMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StartCoroutine(RollToTheLeft());
        movingLeft = true;
    }

    IEnumerator RollToTheLeft()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(rateOfRoll, Vector3.forward);
            
            yield return null;
        }
    }

    IEnumerator RollToTheRight()
    {
        for (float i = 0f; i < amountToRollInDegrees; i += rateOfRoll)
        {
            transform.rotation *= Quaternion.AngleAxis(-rateOfRoll, Vector3.forward);

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if(movingRight && !dead)
        {
            if (rb.velocity.x < 0)
            {
                rb.AddForce(Vector3.right * compensatingLateralForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.right * lateralForce, ForceMode.Acceleration);
        }
        if(movingLeft && !dead)
        {
            if(rb.velocity.x > 0)
            {
                rb.AddForce(Vector3.left * compensatingLateralForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.left * lateralForce, ForceMode.Acceleration);
        }
        if(!movingLeft && !movingRight)
        {
            // Decelerate depending if we are currently moving left or right
            if(rb.velocity.x > 0)
            {
                DecelerateRightwardMovement();
            }
            if(rb.velocity.x < 0)
            {
                DecelerateLeftwardMovement();
            }
        }
        if(rb.useGravity && rb.velocity.y < 0 && !dead)
        {
            // If we are falling after having taken a jump, apply a slight upward force to simulate a gliding effect
            rb.AddForce(Vector3.up * glidingUpwardForce, ForceMode.Force);
        }
    }

    private void LateUpdate()
    {
        shipSparksTransform.position = this.transform.position;
    }

    void DecelerateRightwardMovement()
    {
        if (rb.velocity.x < movingSlowlyThreshold)
        {
            // If we are just barely moving, stop the plane's lateral movement
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            // We are moving to the right so decelerate by applying force to the left
            rb.AddForce(Vector3.left * decelerationForce, ForceMode.Acceleration);
        }
    }

    void DecelerateLeftwardMovement()
    {
        if (rb.velocity.x > -movingSlowlyThreshold)
        {
            // If we are just barely moving, stop the plane's lateral movement
            rb.velocity = new Vector3(0f, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            // We are moving to the right so decelerate by applying force to the left
            rb.AddForce(Vector3.right * decelerationForce, ForceMode.Acceleration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!dead)
        {
            UnityEngine.Debug.Log("Triggery trigger: " + other.tag);
            if (other.tag == Tags.Ramp && rb.velocity.y == 0)
            {
                // Tilt plane back then slowly back down and give it an upward push
                // Be sure to enable gravity before applying the jump so that the plane can fall back down
                StartCoroutine(PitchUpQuicklyAndThenSlowlyBackDown());
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                rb.useGravity = true;
            }
            if (other.tag == Tags.Coin)
            {
                // TODO Collect points
                this.numberOfCoinsCollected++;
                UnityEngine.Debug.Log("Collected coin! Total: " + this.numberOfCoinsCollected);
                Destroy(other.gameObject);
            }
            if (other.tag == Tags.Obstacle)
            {
                // Shake camera
                ShakeCameraDueToImpact();

                // Take damage
                TakeDamage();
            }
            if(other.tag == Tags.Boundary)
            {
                // Show a blur effect to simulate dizziness
                StartCoroutine(ShowDizzinessBlur());

                // Give ship a push back to the track
                rb.AddForce(Vector3.right * -(rb.velocity.x * 1.25f), ForceMode.Impulse);

                // Shake camera
                ShakeCameraDueToImpact();

                // Take some damage and show electrical sparks for some time
                TakeDamage();
                boundaryCollisionSparks.SetActive(true);

                Invoke("TurnOffBoundaryCollisionSparks", 2.5f);
            }
        }   
    }

    private IEnumerator ShowDizzinessBlur()
    {
        blurVolume.gameObject.SetActive(true);
        var maxBlur = 300f;
        var noBlur = 120f;
        var durationInSeconds = 5;
        for(var timePassed = 0f; timePassed < durationInSeconds; timePassed += Time.deltaTime)
        {
            var factor = timePassed / durationInSeconds;
            dizzinessBlur.focalLength.Override(Mathf.Lerp(maxBlur, noBlur, factor));
            yield return null;
        }
        blurVolume.gameObject.SetActive(false);
    }

    private void TurnOffBoundaryCollisionSparks()
    {
        boundaryCollisionSparks.SetActive(false);
    }

    private void ShakeCameraDueToImpact()
    {
        var shakeForce = this.cameraShaker.m_AmplitudeGain < 0.5 ? 1 : this.cameraShaker.m_AmplitudeGain * 3;
        collisionImpulseSource.GenerateImpulse(new Vector3(shakeForce, shakeForce, 0));
    }

    private void TakeDamage()
    {
        health--;
        UnityEngine.Debug.Log("Took a hit! Current health: " + health);
        if (health == 2)
        {
            lightSmoke.SetActive(true);
        }
        if (health == 1)
        {
            // Show smoke and repeatedly show some sparks
            moreSmoke.SetActive(true);
            sparks1.SetActive(true);
            sparks2.SetActive(true);
            sparks3.SetActive(true);
            sparks4.SetActive(true);
            InvokeRepeating("FireFirstSparks", 1, 3);
            InvokeRepeating("FireSecondSparks", 2, 3);
            InvokeRepeating("FireThirdSparks", 3, 3);
            InvokeRepeating("FireFourthSparks", 4, 3);
        }
        if (health == 0)
        {
            UnityEngine.Debug.Log("GAME OVER");
            dead = true;
            // Game Over
            // TODO Crash ship
            rb.useGravity = true;
            rb.AddTorque(new Vector3(0.0f, 2.0f, 0.0f), ForceMode.Impulse);
            cylinder.GetComponent<Cylinder>().ComeToAStop();

            // Turn off camera shake
            this.cameraShaker.m_AmplitudeGain = 0;
        }
    }

    void FireFirstSparks()
    {
        sparks1.GetComponent<ParticleSystem>().Play();
    }

    void FireSecondSparks()
    {
        sparks2.GetComponent<ParticleSystem>().Play();
    }

    void FireThirdSparks()
    {
        sparks3.GetComponent<ParticleSystem>().Play();
    }

    void FireFourthSparks()
    {
        sparks4.GetComponent<ParticleSystem>().Play();
    }

    IEnumerator PitchUpQuicklyAndThenSlowlyBackDown()
    {
        // Quickly pitch plane upwards
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfUpwardPitch)
        {
            transform.Rotate(Vector3.left, rateOfUpwardPitch, Space.World);

            yield return null;
        }

        // Slowly pitch it back down
        for (float i = 0f; i < amountToPitchInDegrees; i += rateOfDownwardPitch)
        {
            transform.Rotate(Vector3.left, -rateOfDownwardPitch, Space.World);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (rb.velocity.y < 0 && rb.useGravity && rb.position.y < cruisingYPos)
            {
                UnityEngine.Debug.Log("BELOW CRUISING!");
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.angularVelocity = Vector3.zero;
                rb.MovePosition(new Vector3(rb.position.x, cruisingYPos, rb.position.z));
            }

            // Since the cylinder rotates ever faster, we want to increase the lateral force as well so that the plane can move sideways more quickly as well over time
            compensatingLateralForce += Cylinder.accelerationFactor;
            lateralForce += Cylinder.accelerationFactor;

            // Slowly start to add some camera shake as we speed up to increase that sense of speed
            var lowestShakingSpeedThreshold = 47;
            var middleShakingSpeedThreshold = 48;
            var highestShakingSpeedThreshold = 49;  // Point after which camera shake is no longer increased any further
            if(Mathf.Abs(cylinderScript.RotationSpeed) > lowestShakingSpeedThreshold && Mathf.Abs(cylinderScript.RotationSpeed) < middleShakingSpeedThreshold)
            {
                if(this.cameraShaker.m_AmplitudeGain == 0)
                {
                    this.cameraShaker.m_AmplitudeGain = 0.5f;
                }
                this.cameraShaker.m_AmplitudeGain += (Cylinder.accelerationFactor / 2);
            }
            else if (Mathf.Abs(cylinderScript.RotationSpeed) > middleShakingSpeedThreshold && Mathf.Abs(cylinderScript.RotationSpeed) < highestShakingSpeedThreshold)
            {
                this.cameraShaker.m_AmplitudeGain += (Cylinder.accelerationFactor / 10);
            }
            
        } else
        {
            if (rb.velocity.y < 0 && rb.useGravity && rb.position.y < crashedYPos)
            {
                UnityEngine.Debug.Log("BELOW CRASHING!");
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.angularVelocity = Vector3.zero;
                rb.MovePosition(new Vector3(rb.position.x, crashedYPos, rb.position.z));
            }
        }
    }
}
