using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Spawnable
{
    private GameObject shockwaveEffect;
    public GameObject explosion;
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
        var newExplosion = Instantiate(explosion);
        newExplosion.transform.position = this.transform.position;
        Destroy(this.gameObject);

        shockwaveEffect.GetComponent<Shockwave>().TriggerShockwave(this.transform.position);
    }
}
