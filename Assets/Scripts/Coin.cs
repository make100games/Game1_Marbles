using UnityEngine;
using System.Collections;

public class Coin : Spawnable
{
    private void FixedUpdate()
    {
        // Have coin spin
        transform.Rotate(0, 10f * Time.deltaTime, 0);
    }
}

