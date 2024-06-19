using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This object should be removed once it has lived a certain amount of time
// and it is currently not in view of the player
public class Obstacle : Spawnable
{
    private Rigidbody rb;

    // The target transform towards which the obstacle will fall. This makes
    // sure that the obstacle always falls "down" no matter where along the surface
    // of the cylinder it is spawned
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    private void FixedUpdate()
    {
        FallTowardsCenterOfCylinder();
    }

    private void FallTowardsCenterOfCylinder()
    {
        // Calculate the direction vector from the falling object to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Apply a force towards the target
        rb.AddForce(direction * (Physics.gravity.magnitude * 2));
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
