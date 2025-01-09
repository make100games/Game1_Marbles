using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScoreKeeper : MonoBehaviour
{
    public long Score { get; private set; }
    private bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartPlaying()
    {
        playing = true;
        Score = 0L;
        StartCoroutine(IncrementEveryHalfSecond());
    }

    public void StopPlaying()
    {
        playing = false;
    }

    private IEnumerator IncrementEveryHalfSecond()
    {
        while (playing)
        {
            Score += 1;

            // Wait for half a second
            yield return new WaitForSeconds(0.5f);
        }
    }
}
