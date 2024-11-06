using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Bomb : Spawnable
{
    private GameObject shockwaveEffect;
    public GameObject explosion;    // The physical explosion that grows and pushes other objects away (more like the shockwave, I guess)
    public GameObject explosionEffect;  // The visual explosion effect
    public float timeTillDetonation = 3f;
    private GameObject mainCamera;
    private Rigidbody rb;
    private Renderer renderer;
    private GameObject explosionInstance;
    private GameObject explosionEffectInstance;

    // Start is called before the first frame update
    void Start()
    {
        shockwaveEffect = GameObject.FindGameObjectWithTag(Tags.ShockwaveEffect);
        mainCamera = GameObject.FindGameObjectWithTag(Tags.MainCamera);
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        Invoke("Explode", timeTillDetonation);
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Physics.gravity.magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        if(explosionInstance != null)
        {
            explosionInstance.transform.position = this.transform.position;
        }
        if(explosionEffectInstance != null)
        {
            explosionEffectInstance.transform.position = this.transform.position;
        }
    }

    void Explode()
    {
        Debug.Log("Kaboom!");
        var isBombVisibleToPlayer = IsBombVisibleToPlayer();
        var positionOfBombAtTimeOfExplosion = this.transform.position;  // Hold for shockwave later on
        GetComponent<MeshRenderer>().enabled = false;
        explosionInstance = Instantiate(explosion);
        explosionEffectInstance = Instantiate(explosionEffect);
        explosionEffectInstance.GetComponent<ExplosionEffect>().OnExplosionFinished += () =>
        {
            // It appears as though the explosion is not rendered onto the camera's opaque texture
            // We therefore should wait with the shockwave until the explosion has finished
            // Only show the shockwave if the bomb is visible to the player. Otherwise we get shockwaves
            // and the player doesnt know why which can get annoying
            if(isBombVisibleToPlayer)
            {
                //shockwaveEffect.GetComponent<Shockwave>().TriggerShockwave(positionOfBombAtTimeOfExplosion);
            }
            Destroy(this.gameObject);
        };
        //newExplosionEffect.transform.position = this.transform.position;
    }

    private bool IsBombVisibleToPlayer()
    {
        return IsObjectVisible(this.transform.position);
    }

    private bool IsObjectVisible(Vector3 objectPosition)
    {
        Vector3 screenPoint = mainCamera.GetComponent<Camera>().WorldToViewportPoint(objectPosition);

        // Check if the object is within the camera's viewport
        bool isInView = screenPoint.z > 0 &&
                        screenPoint.x > 0 && screenPoint.x < 1 &&
                        screenPoint.y > 0 && screenPoint.y < 1;

        if (isInView)
        {
            // Cast a ray from the camera to the object and check for obstacles. We use RaycastAll because we don't simply want to know about
            // the first object the ray hit but any object in its path up to a certain distance (maxDistance)
            var maxDistance = 100f;
            RaycastHit[] hits = Physics.RaycastAll(mainCamera.transform.position, objectPosition - mainCamera.transform.position, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if(hit.transform == this.gameObject.transform)
                {
                    return true;
                }
            }
            return false;
        }

        return false;
    }
}
