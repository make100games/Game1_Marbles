using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is in charge of turning on the various waves of spawners as the
/// game progresses to increase difficulty.
/// </summary>
public class SpawnerCoordinator : MonoBehaviour
{
    /// <summary>
    /// The waves of spawners. Each wave contains one or more spawners that will
    /// become active when the wave becomes active.
    /// </summary>
    public List<GameObject> waves;

    /// <summary>
    /// The duration in sesconds during which a wave is active. After this time elapses, the
    /// current wave will become disabled and the next wave will become enabled.
    /// </summary>
    public int durationOfEachWave = 10;

    private int indexOfCurrentWave = 0;
    private GameObject currentWave = null;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ActivateNextWave", 0, durationOfEachWave);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ActivateNextWave()
    {
        if (currentWave != null)
        {
            currentWave.SetActive(false);
        }
        currentWave = waves[indexOfCurrentWave];
        currentWave.SetActive(true);
        indexOfCurrentWave++;
    }
}
