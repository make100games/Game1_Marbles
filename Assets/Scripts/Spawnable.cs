using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This object should be removed once it has lived a certain amount of time
public class Spawnable : MonoBehaviour
{
    private float timeToLive = 5f;  // Time until this obstacle is removed

    // Update is called once per frame
    void Update()
    {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
