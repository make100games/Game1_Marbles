using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This light changes its intensity as a function of a rotating body.
/// </summary>
public class DynamicAmbientLight : MonoBehaviour
{
    public GameObject rotatingBody;
    private Light _light;

    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        // The intensity of the light is a function of the sine of the rotating
        // body's angle around the X axis.
        // Get the Euler angles of the GameObject
        Vector3 rotation = rotatingBody.transform.eulerAngles;

        // Convert to radians
        float rotationX = rotation.x * Mathf.Deg2Rad;
        
        // Calculate sine and cosine
        float sinX = Mathf.Sin(rotationX);

        var newIntensity = -(sinX) * 0.7f;
        if(newIntensity < 0.2)
        {
            newIntensity = 0.2f;
        }
        if(newIntensity > 0.7)
        {
            newIntensity = 0.7f;
        }
        _light.intensity = newIntensity;
    }
}
