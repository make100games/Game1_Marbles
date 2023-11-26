using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCraft : MonoBehaviour
{
    public float rotationSpeed = 15f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // We want to decrease the speed of rotation of the non-playable aircraft over
        // time because we want them to get slower relative to the player as the player
        // speeds up
        rotationSpeed -= Cylinder.accelerationFactor;
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
