using UnityEngine;
using System.Collections;

public class FullscreenCameraEffect : MonoBehaviour
{
    public Camera targetCamera;
    public float distanceFromCamera = 10f;

    protected void FitToCameraView()
    {
        // Get the aspect ratio and FOV of the camera
        float aspectRatio = targetCamera.aspect;
        float fov = targetCamera.fieldOfView;

        // Calculate the height at the given distance from the camera
        float heightAtDistance = 2.0f * distanceFromCamera * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

        // Calculate the width based on the aspect ratio
        float widthAtDistance = heightAtDistance * aspectRatio;

        // Scale the plane to match the size
        transform.localScale = new Vector3(widthAtDistance / 10f, 1f, heightAtDistance / 10f);  // Default plane is 10x10 in Unity

        // Position the plane at the given distance from the camera
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distanceFromCamera;

        // Ensure the plane faces the camera
        //transform.rotation = Quaternion.LookRotation(-targetCamera.transform.forward);
    }
}

