using UnityEngine;
using System.Collections;

public class Coin : Spawnable
{
    private ParticleSystem collectedParticles;

    private void Start()
    {
        collectedParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        
    }
}

