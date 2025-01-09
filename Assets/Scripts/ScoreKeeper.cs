using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScoreKeeper : MonoBehaviour
{
    public TMP_Text runningScoreText;
    private long score;
    public long Score
    {
        get => score;
        private set
        {
            score = value;
            UpdateScoreText();
        }
    }
    private bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateScoreText()
    {
        if (runningScoreText != null)
        {
            runningScoreText.text = "Score: " + Score.ToString();
        }
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
