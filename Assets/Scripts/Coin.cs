using UnityEngine;
using System.Collections;

public class Coin : Spawnable
{
    private ParticleSystem collectedParticles;
    private GameObject spinner;

    private void Start()
    {
        collectedParticles = GetComponentInChildren<ParticleSystem>();
        spinner = GameObject.FindGameObjectWithTag(Tags.CoinSpinner);
    }

    private void Update()
    {
        this.transform.rotation = spinner.transform.rotation;
    }

    private void FixedUpdate()
    {
        
    }
}

