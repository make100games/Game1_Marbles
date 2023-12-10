using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object should be removed once it has lived a certain amount of time
public class Spawnable : MonoBehaviour
{
    public float timeToLive = 5f;  // Time until this obstacle is removed
    public bool inDeletionZone = false;

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0 && inDeletionZone)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tags.DeletionZone)
        {
            inDeletionZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == Tags.DeletionZone)
        {
            inDeletionZone = false;
        }
    }
}
