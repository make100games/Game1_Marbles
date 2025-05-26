using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays a whoosh sound effect when objects pass near the ship.
/// </summary>
public class WhooshEffect : MonoBehaviour
{
    public Transform shipTransform;
    public bool Dead { get; set; } // True if the player has crashed the plane

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = shipTransform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Dead)
        {
            if (other.tag == Tags.Obstacle)
            {
                GetComponent<AudioSource>().Play();
            }
            
        }
    }
}
