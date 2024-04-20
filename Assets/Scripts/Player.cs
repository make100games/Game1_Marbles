using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the position of the coin collection particle effect to that of the ship.
/// </summary>
public class Player : MonoBehaviour
{
    public List<GameObject> coinCollectedParticleEffectObjects = new List<GameObject>();
    public GameObject ship;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in coinCollectedParticleEffectObjects)
        {
            obj.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, ship.transform.position.z);
        }
    }
}
