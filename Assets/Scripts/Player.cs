using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the position of the coin collection particle effect to that of the ship.
/// </summary>
public class Player : MonoBehaviour
{
    public GameObject coinCollectionParticleEffectObject;
    public GameObject ship;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coinCollectionParticleEffectObject.transform.position = ship.transform.position;
    }
}
