using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

public class Plane : MonoBehaviour
{
    public GameObject scoreKeeper;
    public Transform shipSparksTransform;   // Used by the electric sparks particle system so that it follows the ship but does not rotate with the ship. Can be just some transform in the scene (e.g. an empty GameObject)
    public GameObject cylinder; // The cylinder around which the plane is flying. Technically the plane is standing still and the cylinder is spinning but you know what I mean
    public GameObject electricalSparks;  // The sparks which appear when the ship has been hit twice
    public GameObject lightSmoke;   // The smoke to show when plane has been hit once
    public GameObject moreSmoke;    // Smoke to show when plane has been hit twice
    public GameObject boostThrusters;   //The thrusters that are enabled while we are boosting
    public GameObject trackingCamera;   // The camera tracking the plane
    public Volume blurVolume;
    public GameObject coinCollectionStarsParticleEffectObject;
    public GameObject coinCollectionBlobsParticleEffectObject;
    public GameObject coinCollectedLargeParticleEffectObject;
    public GameObject coinCollectSoundEffect;
    public GameObject collisionSoundEffect1;
    public GameObject collisionSoundEffect2;
    public GameObject collisionSoundEffect3;
    public GameObject collisionCrashEffect;
    public GameObject thrustEffect;
    public bool Dead { get; private set; } // True if the player has crashed the plane

    public event Action OnPlaneCrashed;

    private Renderer objectRenderer;
    private Cylinder cylinderScript;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin cameraShaker;    // The part of the camera that controls how the camera shakes
    private GameInput gameInput;
    private float amountToRollInDegrees = 45;   // Amount of degrees to roll to the left or right when flying left/right
    private float amountToRollDuringBarellRollInDegrees = 360;  // Amount of degrees to roll to the left or right when doing a barrell roll
    private float rateOfRoll = 120f;   // Amount to roll in a single frame update ignoring Time.deltaTime
    private float rateOfBarrellRoll = 990.0f; // Amount to barrell roll in a single frame update ignoring Time.deltaTime
    private float amountToPitchInDegrees = 45;  // Amount of degrees to pitch plane back when we hit a ramp
    private float rateOfUpwardPitch = 90f;    // 0.25 Amount to pitch in a single frame update when pitching up
    private float rateOfDownwardPitch = 30f;   // 0.0725 Amount to pitch in a single frame update when pitching down
    private Rigidbody rb;
    private bool movingLeft = false;
    private bool movingRight = false;
    private float compensatingLateralForce = 40f;   // Originally 25. Force to compensate for movement in opposite direction. ie: If you are currently moving left and then change directions to move right, we want to apply a bit more force while you are still drifing left so that the plane can correct course more quickly
    private float lateralForce = 60;    // Originally 33. Force to apply when moving left or right
    private float barrelRollLateralForce = 950f; // Force to apply when doing a barrel roll
    private float movingSlowlyThreshold = 0.5f; // Speed below which we consider a plane's lateral movement to be slow
    private const float defaultDecelerationForce = 35f;     // Base force at which to decelerate lateral movement
    private float decelerationForce = defaultDecelerationForce;  // Force at which we decelerate lateral movement when player stops moving laterally
    private float jumpForce = 25f;  // Upward force applied to the plane to make it jump
    private float cruisingYPos; // The y position of the plane when it is just cruising over the surface of the cylinder. This is the y position the plane will come back down to after a jump.
    private float crashedYPos;  // The y position of the plane when it has hit the ground after a crash (game over)
    private float glidingUpwardForce = 20f;  // When we jump, we want to glide back down and not simply fall down. This represents the slight upward force to counteract gravity a bit
    private int numberOfCoinsCollected = 0;
    private int health = 3; // Plane can take 3 hits before crashing
    private CinemachineCollisionImpulseSource collisionImpulseSource;
    private DepthOfField dizzinessBlur;
    private ParticleSystem coinCollectedStarEffect;
    private ParticleSystem coinCollectedBlobEffect;
    private ParticleSystem coinCollectedLargeEffect;
    private bool boostActive = false;
    private bool justBeenHit = false;   // True if ship has just hit an obstacle. Gives the player a few seconds of invincibility
    private float timeInvincibleAfterCollision = 2f;    // How long the ship is invincible after a collision (prevents player from dying by hitting two obstacles that are right next to each other)

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        rb = GetComponent<Rigidbody>();
        cylinderScript = cylinder.GetComponent<Cylinder>();
        virtualCamera = trackingCamera.GetComponent<CinemachineVirtualCamera>();
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
        gameInput.Game.LeftBarrelRoll.performed += LeftBarrelRoll_performed;
        gameInput.Game.RightBarrelRoll.performed += RightBarrelRoll_performed;

        cruisingYPos = transform.position.y;
        crashedYPos = cruisingYPos - 3;

        coinCollectedStarEffect = coinCollectionStarsParticleEffectObject.GetComponent<ParticleSystem>();
        coinCollectedBlobEffect = coinCollectionBlobsParticleEffectObject.GetComponent<ParticleSystem>();
        coinCollectedLargeEffect = coinCollectedLargeParticleEffectObject.GetComponent<ParticleSystem>();
        objectRenderer = GetComponent<Renderer>();
    }

    public void StartPlayingThrusterSoundEffect()
    {
        float targetVolume = 0.4f;
        float rateOfVolumeIncrease = 0.1f;
        this.thrustEffect.GetComponent<AudioSource>().volume = 0;
        this.thrustEffect.GetComponent<AudioSource>().Play();
        StartCoroutine(ChangeThrusterVolume(targetVolume, rateOfVolumeIncrease));
    }

    private IEnumerator ChangeThrusterVolume(float targetVolume, float rateOfVolumeChange)
    {
        if (rateOfVolumeChange > 0) { 
            while (this.thrustEffect.GetComponent<AudioSource>().volume < targetVolume)
            {
                this.thrustEffect.GetComponent<AudioSource>().volume += (rateOfVolumeChange * Time.deltaTime);
                yield return null;
            }
            if (this.thrustEffect.GetComponent<AudioSource>().volume > targetVolume)
            {
                this.thrustEffect.GetComponent<AudioSource>().volume = targetVolume;
            }
        }
        else if(rateOfVolumeChange < 0)
        {
            while (this.thrustEffect.GetComponent<AudioSource>().volume > targetVolume)
            {
                this.thrustEffect.GetComponent<AudioSource>().volume += (rateOfVolumeChange * Time.deltaTime);
                yield return null;
            }
            if (this.thrustEffect.GetComponent<AudioSource>().volume < targetVolume)
            {
                this.thrustEffect.GetComponent<AudioSource>().volume = targetVolume;
            }
        }
    }

    private void ToggleBoost_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        boostActive = !boostActive;
        if(boostActive)
        {
            ApplyBoost();
        }
        else
        {
            StopBoost();
        }
    }

    private void ApplyBoost()
    {
        // No op as no longer used
    }

    private IEnumerator BringBackCamera()
    {
        float elapsedTime = 0f;
        float duration = 2f;    // Duration of interpolation in seconds

        while (elapsedTime < duration)
        {
            // Calculate interpolation parameter based on elapsed time
            float t = elapsedTime / duration;

            // Linearly interpolate between startValue and endValue
            float cameraDistance = Mathf.Lerp(14f, 11f, t);
            float cameraY = Mathf.Lerp(0.45f, 0.6f, t);

            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = cameraDistance;
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = cameraY;

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }

    private void StopBoost()
    {
        cylinder.GetComponent<Cylinder>().StopBoost();
        this.cameraShaker.m_AmplitudeGain -= 1.15f;
        lateralForce -= 20.0f;
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 10;
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.65f;
        boostThrusters.SetActive(false);
    }

    private void RightBarrelRoll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!this.Dead)
        {
            rb.AddForce(Vector3.right * barrelRollLateralForce, ForceMode.Force);
            StartCoroutine(BarrelRollToTheRight());
        }
    }

    private void LeftBarrelRoll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!this.Dead)
        {
            rb.AddForce(Vector3.left * barrelRollLateralForce, ForceMode.Force);
            StartCoroutine(BarrelRollToTheLeft());
        }
    }

    private void StopMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!this.Dead)
        {
            StartCoroutine(RollToTheLeft());
            movingRight = false;
        }
    }

    private void StartMovingRight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!this.Dead)
        {
            StartCoroutine(RollToTheRight());
            movingRight = true;
        }
    }

    private void StopMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!this.Dead)
        {
            StartCoroutine(RollToTheRight());
            movingLeft = false;
        }
    }

    private void StartMovingLeft_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(!this.Dead)
        {
            StartCoroutine(RollToTheLeft());
            movingLeft = true;
        }
    }

    private IEnumerator BarrelRollToTheLeft()
    {
        float totalRolled = 0f; // Tracks the total degrees rolled so far

        while (totalRolled < amountToRollDuringBarellRollInDegrees)
        {
            // Calculate the rotation for this frame based on the rate and deltaTime
            float rotationThisFrame = rateOfBarrellRoll * Time.deltaTime;

            // Ensure we don't over-rotate past the target
            rotationThisFrame = Mathf.Min(rotationThisFrame, amountToRollDuringBarellRollInDegrees - totalRolled);

            // Apply the rotation
            transform.rotation *= Quaternion.AngleAxis(rotationThisFrame, Vector3.forward);

            // Update the total amount rolled
            totalRolled += rotationThisFrame;

            yield return null;
        }
    }

    private IEnumerator BarrelRollToTheRight()
    {
        float totalRolled = 0f; // Tracks the total degrees rolled so far

        while (totalRolled < amountToRollDuringBarellRollInDegrees)
        {
            // Calculate the rotation for this frame based on the rate and deltaTime
            float rotationThisFrame = rateOfBarrellRoll * Time.deltaTime;

            // Ensure we don't over-rotate past the target
            rotationThisFrame = Mathf.Min(rotationThisFrame, amountToRollDuringBarellRollInDegrees - totalRolled);

            // Apply the rotation
            transform.rotation *= Quaternion.AngleAxis(-rotationThisFrame, Vector3.forward);

            // Update the total amount rolled
            totalRolled += rotationThisFrame;

            yield return null;
        }
    }

    private IEnumerator RollToTheLeft()
    {
        float totalRolled = 0f; // Tracks the total degrees rolled so far

        while (totalRolled < amountToRollInDegrees)
        {
            // Calculate the rotation for this frame based on the roll rate and deltaTime
            float rotationThisFrame = rateOfRoll * Time.deltaTime;

            // Ensure we don't over-rotate past the target
            rotationThisFrame = Mathf.Min(rotationThisFrame, amountToRollInDegrees - totalRolled);

            // Apply the rotation
            transform.rotation *= Quaternion.AngleAxis(rotationThisFrame, Vector3.forward);

            // Update the total amount rolled
            totalRolled += rotationThisFrame;

            yield return null;
        }
    }

    IEnumerator RollToTheRight()
    {
        float totalRolled = 0f; // Tracks the total degrees rolled so far

        while (totalRolled < amountToRollInDegrees)
        {
            // Calculate the rotation for this frame based on the roll rate and deltaTime
            float rotationThisFrame = rateOfRoll * Time.deltaTime;

            // Ensure we don't over-rotate past the target
            rotationThisFrame = Mathf.Min(rotationThisFrame, amountToRollInDegrees - totalRolled);

            // Apply the rotation
            transform.rotation *= Quaternion.AngleAxis(-rotationThisFrame, Vector3.forward);

            // Update the total amount rolled
            totalRolled += rotationThisFrame;

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        // Increase deceleration force to compensate for increased lateral force
        decelerationForce = defaultDecelerationForce + Mathf.Abs(rb.velocity.x);
        if(movingRight && !Dead)
        {
            if (rb.velocity.x < 0)
            {
                rb.AddForce(Vector3.right * compensatingLateralForce, ForceMode.Acceleration);
            }
            rb.AddForce(Vector3.right * lateralForce, ForceMode.Acceleration);
        }
        if(movingLeft && !Dead)
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
        if(rb.useGravity && rb.velocity.y < 0 && !Dead)
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
        if(!Dead)
        {
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
                this.coinCollectSoundEffect.GetComponent<AudioSource>().Play();
                this.scoreKeeper.GetComponent<ScoreKeeper>().NrOfCoinsCollected++;

                // Show particle effect to indicate that coin was collected
                coinCollectedStarEffect.Play();
                coinCollectedBlobEffect.Play();
                coinCollectedLargeEffect.Emit(1);
                coinCollectedLargeEffect.Play();

                Destroy(other.gameObject);
            }
            if (other.tag == Tags.Obstacle && !boostActive && !justBeenHit)
            {
                collisionCrashEffect.GetComponent<AudioSource>().Play();

                // Shake camera
                ShakeCameraDueToImpact();

                // Take damage
                TakeDamage();
            }
            if(other.tag == Tags.Boundary)
            {
                // Since we still want the ship to be pushed back to the track when hitting the boundary even if it has just been hit
                // we do this check in here. This makes sure the ship doesn't take any damage if it has just been hit and crashed into the
                // boundary but still ensures the ship doesn't just pass through
                if(!justBeenHit)
                {
                    collisionCrashEffect.GetComponent<AudioSource>().Play();

                    // Shake camera
                    ShakeCameraDueToImpact();

                    // Take some damage
                    TakeDamage();
                }
                
                // Give ship a push back to the track
                rb.AddForce(Vector3.right * -(rb.velocity.x * 1.25f), ForceMode.Impulse);
            }
        }
        else
        {
            if (other.tag == Tags.Boundary)
            {
                // Give ship a push back to the track
                rb.AddForce(Vector3.right * -(rb.velocity.x * 1.25f), ForceMode.Impulse);
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

    private void ShakeCameraDueToImpact()
    {
        var shakeForce = this.cameraShaker.m_AmplitudeGain < 0.5 ? 1 : this.cameraShaker.m_AmplitudeGain * 3;
        collisionImpulseSource.GenerateImpulse(new Vector3(shakeForce, shakeForce, 0));
    }

    private void TakeDamage()
    {
        health--;
        if(health >= 1)
        {
            if(health >= 2)
            {
                collisionSoundEffect1.GetComponent<AudioSource>().Play();
            }
            else if(health >= 1)
            {
                collisionSoundEffect2.GetComponent<AudioSource>().Play();
            }
            
            MakePlaneTemporarilyInvincible(health);
        }
        else if (health == 0)
        {
            collisionSoundEffect3.GetComponent<AudioSource>().Play();
            Dead = true;
            // Game Over
            rb.useGravity = true;
            rb.AddTorque(new Vector3(0.0f, 2.0f, 0.0f), ForceMode.Impulse);
            cylinder.GetComponent<Cylinder>().ComeToAStop();

            // Turn off camera shake
            this.cameraShaker.m_AmplitudeGain = 0;

            this.OnPlaneCrashed?.Invoke();

            StartCoroutine(ChangeThrusterVolume(0.0f, -0.1f));
        }
    }

    private void MakePlaneTemporarilyInvincible(int health)
    {
        justBeenHit = true;
        objectRenderer.material.SetInt("_IsFlashing", 1);
        objectRenderer.material.SetFloat("_Speed", 5f);
        objectRenderer.material.SetColor("_Color", Color.cyan);

        StartCoroutine(ShowEffectsOfDamageAfterDelay(timeInvincibleAfterCollision, health));
    }

    /// <summary>
    /// Shows the effects of the damage taken after a delay.
    /// </summary>
    /// <param name="delay">Delay in seconds after which to show the effects of the damage</param>
    /// <param name="health">Current health after damage was taken. Determines how the current level of damage is visualized</param>
    /// <returns>Irrelephant. Needs to be started in a coroutine so that it can do its thing after a delay</returns>
    private IEnumerator ShowEffectsOfDamageAfterDelay(float delay, int health)
    {
        yield return new WaitForSeconds(delay);

        justBeenHit = false;
        objectRenderer.material.SetInt("_IsFlashing", 0);   // Stop the flashing effect that indicates ship is temporarily invincible after collision

        if (health == 2)
        {
            lightSmoke.SetActive(true);
        }
        if (health == 1)
        {
            // Show smoke and repeatedly show some sparks. Also, make the ship flash red
            moreSmoke.SetActive(true);
            electricalSparks.SetActive(true);
            objectRenderer.material.SetInt("_IsFlashing", 1);
            objectRenderer.material.SetFloat("_Speed", 1f);
            objectRenderer.material.SetColor("_Color", Color.red);
        }
    }

    private IEnumerator PitchUpQuicklyAndThenSlowlyBackDown()
    {
        float totalPitchedUp = 0f;

        // Quickly pitch the plane upwards
        while (totalPitchedUp < amountToPitchInDegrees)
        {
            // Calculate pitch for this frame based on upward pitch rate
            float pitchThisFrame = rateOfUpwardPitch * Time.deltaTime;

            // Ensure we don't over-pitch
            pitchThisFrame = Mathf.Min(pitchThisFrame, amountToPitchInDegrees - totalPitchedUp);

            // Apply the pitch
            transform.Rotate(Vector3.left, pitchThisFrame, Space.World);

            // Update the total amount pitched up
            totalPitchedUp += pitchThisFrame;

            yield return null;
        }

        float totalPitchedDown = 0f;

        // Slowly pitch the plane back down
        while (totalPitchedDown < amountToPitchInDegrees)
        {
            // Calculate pitch for this frame based on downward pitch rate
            float pitchThisFrame = rateOfDownwardPitch * Time.deltaTime;

            // Ensure we don't over-pitch back down
            pitchThisFrame = Mathf.Min(pitchThisFrame, amountToPitchInDegrees - totalPitchedDown);

            // Apply the pitch
            transform.Rotate(Vector3.left, -pitchThisFrame, Space.World);

            // Update the total amount pitched down
            totalPitchedDown += pitchThisFrame;

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            if (rb.velocity.y < 0 && rb.useGravity && rb.position.y < cruisingYPos)
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.angularVelocity = Vector3.zero;
                rb.MovePosition(new Vector3(rb.position.x, cruisingYPos, rb.position.z));
            }

            // Since the cylinder rotates ever faster, we want to increase the lateral force as well so that the plane can move sideways more quickly as well over time
            compensatingLateralForce += (Cylinder.accelerationFactor * Time.deltaTime);
            lateralForce += (Cylinder.accelerationFactor * Time.deltaTime);

            // Slowly start to add some camera shake as we speed up to increase that sense of speed
            var lowestShakingSpeedThreshold = 55;   // Originally 47
            var middleShakingSpeedThreshold = 56;   // Originally 48
            var highestShakingSpeedThreshold = 57;  // Originally 49. Point after which camera shake is no longer increased any further
            if(Mathf.Abs(cylinderScript.RotationSpeedIgnoringBoost) > lowestShakingSpeedThreshold && Mathf.Abs(cylinderScript.RotationSpeedIgnoringBoost) < middleShakingSpeedThreshold)
            {
                if(this.cameraShaker.m_AmplitudeGain == 0)
                {
                    this.cameraShaker.m_AmplitudeGain = 0.5f;
                }
                this.cameraShaker.m_AmplitudeGain += ((Cylinder.accelerationFactor / 2) * Time.deltaTime);
            }
            else if (Mathf.Abs(cylinderScript.RotationSpeedIgnoringBoost) > middleShakingSpeedThreshold && Mathf.Abs(cylinderScript.RotationSpeedIgnoringBoost) < highestShakingSpeedThreshold)
            {
                this.cameraShaker.m_AmplitudeGain += ((Cylinder.accelerationFactor / 10) * Time.deltaTime);
            }
            
        } else
        {
            if (rb.velocity.y < 0 && rb.useGravity && rb.position.y < crashedYPos)
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.angularVelocity = Vector3.zero;
                rb.MovePosition(new Vector3(rb.position.x, crashedYPos, rb.position.z));
            }
        }
    }
}
