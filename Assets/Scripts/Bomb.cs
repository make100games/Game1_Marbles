using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Spawnable
{
    private GameObject shockwaveEffect;
    public GameObject explosion;    // The physical explosion that grows and pushes other objects away (more like the shockwave, I guess)
    public GameObject explosionEffect;  // The visual explosion effect
    public float timeTillDetonation = 3f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        shockwaveEffect = GameObject.FindGameObjectWithTag(Tags.ShockwaveEffect);
        rb = GetComponent<Rigidbody>();
        Invoke("Explode", timeTillDetonation);
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Physics.gravity.magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Explode()
    {
        Debug.Log("Kaboom!");
        var positionOfBombAtTimeOfExplosion = this.transform.position;  // Hold for shockwave later on
        GetComponent<MeshRenderer>().enabled = false;
        var newExplosion = Instantiate(explosion);
        newExplosion.transform.position = this.transform.position;
        var newExplosionEffect = Instantiate(explosionEffect);
        newExplosionEffect.GetComponent<ExplosionEffect>().OnExplosionFinished += () =>
        {
            // It appears as though the explosion is not rendered onto the camera's opaque texture
            // We therefore should wait with the shockwave until the explosion has finished
            Debug.Log("Explosion finished playing. Show shockwave and kill bomb object!");
            shockwaveEffect.GetComponent<Shockwave>().TriggerShockwave(positionOfBombAtTimeOfExplosion);
            Destroy(this.gameObject);
        };
        newExplosionEffect.transform.position = this.transform.position;
    }
}
