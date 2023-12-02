using UnityEngine;
using System.Collections;

public class Coin : Spawnable
{
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
			
	}

    private void FixedUpdate()
    {
        // Have coin spin
        transform.Rotate(0, 10f * Time.deltaTime, 0);
    }
}

