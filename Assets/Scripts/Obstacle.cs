using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This object should be removed once it has lived a certain amount of time
// and it is currently not in view of the player
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

    public void DestroyAfterAShortWhile()
    {
        Invoke("RemoveObstacle", 5f);
    }

    private void RemoveObstacle()
    {
        Destroy(this.gameObject);
    }
}
