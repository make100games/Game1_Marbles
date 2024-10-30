using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(25f, 25f, 25f); // Target scale
    public float duration = 5.5f; // Duration over which to scale                                  
    public float forceMagnitude = 10f; // Force magnitude to apply to the entering object

    private Vector3 initialScale;
    private float timeElapsed = 0f;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float lerpFactor = timeElapsed / duration;
        transform.localScale = Vector3.Lerp(initialScale, targetScale, lerpFactor);

        // Ensure the scaling stops once the target scale is reached
        if (lerpFactor >= 1f)
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
