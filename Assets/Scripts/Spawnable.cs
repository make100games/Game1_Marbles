using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Tags.DeletionZone)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == Tags.DeletionZone)
        {
            Destroy(this.gameObject);
        }
    }
}
