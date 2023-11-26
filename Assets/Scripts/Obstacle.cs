using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: This object should be removed once it has lived a certain amount of time
// and it is currently not in view of the player
public class Obstacle : MonoBehaviour
{
    private float timeToLive = 5f;  // Time until this obstacle is removed
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();    
    }

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if(timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Physics.gravity.magnitude);
    }
}
