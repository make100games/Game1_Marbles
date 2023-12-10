using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FIXME: If an object is never going to enter deletion zone, it will stick around forever.
//  An example of this is an obstacle which increases its orbit over time and eventually never crosses
//  deletion zone again. To fix this: if an object is so old that it must have crossed through
//  deletion zone at least 2 times already, delete it anyway. That means it is most likely out of sight of
//  the player anyway (e.g. some obstacle that has been hurled out into space).
// This object should be removed once it has lived a certain amount of time
public class Spawnable : MonoBehaviour
{
    public float timeToLive = 5f;  // Time until this obstacle is removed
    private bool inDeletionZone = false;

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
