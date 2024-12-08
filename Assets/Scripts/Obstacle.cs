using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Spawnable
{
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Physics.gravity.magnitude);
    }

    private void RemoveObstacle()
    {
        Destroy(this.gameObject);
    }
}
