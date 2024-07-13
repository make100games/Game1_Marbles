using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(20f, 20f, 20f); // Target scale
    public float duration = 0.5f; // Duration over which to scale

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
}
