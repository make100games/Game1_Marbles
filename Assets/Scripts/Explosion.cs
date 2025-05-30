using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(35f, 35f, 35f); // Target scale
    public float duration = 5.5f; // Duration over which to scale                                  
    public float forceMagnitude = 10f; // Force magnitude to apply to the entering object

    private Vector3 initialScale;
    private float timeElapsed = 0f;
    private float dissolutionAmount = 0.0f; // Amount by which explosion should have dissolved
    private Renderer objectRenderer;
    private AudioSource explosionEffect;
    private float maxTimeUntilDestroy = 20; // Time in seconds until we kill explosion (unless it has already been destroyed before)

    void Start()
    {
        initialScale = transform.localScale;
        objectRenderer = GetComponent<Renderer>();
        explosionEffect = GetComponent<AudioSource>();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float lerpFactor = timeElapsed / duration;
        transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpFactor);

        // Dissolve explosion over time
        objectRenderer.material.SetFloat("_ClipThreshold", dissolutionAmount);
        dissolutionAmount += 0.0025f;

        // Ensure the scaling stops once the target scale is reached
        // Destroy the explosion once the scale of it has reached a certain point and the sound
        // effect has fully played
        if ((lerpFactor >= 1f && !explosionEffect.isPlaying && explosionEffect.time > 0) || timeElapsed >= maxTimeUntilDestroy)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != Tags.Player && other.tag != Tags.Explosion && other.tag != Tags.Coin)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calculate the direction from the trigger center to the entering object
                Vector3 direction = (other.transform.position - transform.position).normalized;

                // Apply force to the Rigidbody in the calculated direction
                rb.AddForce(direction * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
