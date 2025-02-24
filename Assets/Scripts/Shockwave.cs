using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Shockwave : FullscreenCameraEffect
{
    private Renderer objectRenderer;
    private GameInput gameInput;
    private float shockwaveProgress = 0.0f;
    private bool shockwaveRunning = false;
    private float lastAspectRatio;
    private float checkInterval = 0.5f;    // Interval in seconds to check for aspect ratio changes

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        FitToCameraView();
        objectRenderer = GetComponent<Renderer>();
        lastAspectRatio = Camera.main.aspect;
        StartCoroutine(CheckForChangeInAspectRatio());

        //gameInput.Game.DebugClick.performed += DebugClick_performed;    // Only for debugging purposes
    }

    private IEnumerator CheckForChangeInAspectRatio()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);  // Wait for the specified interval

            float currentAspectRatio = Camera.main.aspect;
            if (!Mathf.Approximately(lastAspectRatio, currentAspectRatio))
            {
                OnAspectRatioChanged(currentAspectRatio);
                lastAspectRatio = currentAspectRatio;  // Update the last known aspect ratio
            }
        }
    }

    private void OnAspectRatioChanged(float newAspectRatio)
    {
        Debug.Log("Aspect ratio changed to: " + newAspectRatio);

        FitToCameraView();
    }

    private void DebugClick_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        // Get the camera
        Camera cam = Camera.main;

        // Get mouse position
        var mousePos = Mouse.current.position.ReadValue();

        // Create a ray from the camera through the mouse position
        Ray ray = cam.ScreenPointToRay(mousePos);

        // Perform raycast
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // Get the point in the scene where the ray hit
            Vector3 hitPoint = hitInfo.point;

            // Log or use the hit point
            Debug.Log("Clicked position in the scene: " + hitPoint);
            TriggerShockwave(hitPoint);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(shockwaveRunning && shockwaveProgress < 1.0f)
        {
            //shockwaveProgress += 0.0015f;
            shockwaveProgress += 0.0030f;
            objectRenderer.material.SetFloat("_Progress", shockwaveProgress);
        }
        else if(shockwaveProgress >= 1.0f)
        {
            // Shockwave is finished. Disable the renderer. Disabling the object
            // will make it so that it cannot be found anymore by outside objects
            // which is not optimal.
            this.objectRenderer.enabled = false;
        }
    }

    public void TriggerShockwave(Vector3 location)
    {
        var focalPoint = MapWorldPositionToUV(location);
        // If the origin of the shockwave is off screen, the focalPoint
        // will be Vector2.negativeInfinity. In that case, we do not want to
        // draw the shockwave
        if (focalPoint.x >= 0 && focalPoint.y >= 0)
        {
            this.objectRenderer.enabled = true;
            shockwaveProgress = 0f;
            objectRenderer.material.SetFloat("_Progress", 0.0f);
            objectRenderer.material.SetVector("_FocalPoint", focalPoint);
            this.shockwaveRunning = true;
        }   
    }

    private Vector2 MapWorldPositionToUV(Vector3 worldPosition)
    {
        var camera = targetCamera;

        // Convert world position to screen space
        Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

        // Cast a ray from the screen point
        Ray ray = camera.ScreenPointToRay(screenPosition);

        // Check if the ray intersects with the full screen effect plane
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            // Get the world position on the plane
            Vector3 hitPoint = hitInfo.point;

            // Convert hit point to plane's local space (to map UVs or for other purposes)
            // The default unity plane spans from -5 to 5 by default
            Vector3 localPoint = transform.InverseTransformPoint(hitPoint);

            // Convert to normalized UV values (between 0 and 1)
            // Unity's default Plane primitive spans 10 units along both the X and Z axes.
            float planeWidth = 10.0f;   // Scaled width
            float planeHeight = 10.0f;  // Scaled height

            // Calculate UV coordinates
            float u = (localPoint.x + 5.0f) / planeWidth;   // X maps to U. Adding 5 because the plane spans from -5 to 5. We want to calculate from 0 to 10
            float v = (localPoint.z + 5.0f) / planeHeight;  // Z maps to V. Adding 5 because the plane spans from -5 to 5. We want to calculate from 0 to 10

            // Ensure UVs are in [0, 1] range
            u = Mathf.Clamp01(u);
            v = Mathf.Clamp01(v);

            // UV coordinates for the hit point
            Vector2 uv = new Vector2(u, v);
            return uv;
        }
        return Vector2.negativeInfinity;    // error case
    }
}
