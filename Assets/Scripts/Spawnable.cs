using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public float timeToLive = 5f;  // Time until this obstacle is removed (in seconds)
    private float maxTimeToLive = 30f;  // Time after which something gest deleted regardless of where it is
    private float timeLived = 0;
    private bool inDeletionZone = false;

    // Update is called once per frame
    void Update()
    {
        timeLived += Time.deltaTime;
        if ((timeLived >= timeToLive && inDeletionZone) || timeLived >= maxTimeToLive)
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
