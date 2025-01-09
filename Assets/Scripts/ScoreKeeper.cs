using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScoreKeeper : MonoBehaviour
{
    private long score;
    private int nrOfCoinsCollected;
    private bool playing = false;
    private const string HighScoreKey = "HighScore";

    public TMP_Text runningScoreText;
    public TMP_Text coinsText;

    public long Score
    {
        get => score;
        private set
        {
            score = value;
            UpdateScoreText();
        }
    }

    public int NrOfCoinsCollected
    {
        get => nrOfCoinsCollected;
        set
        {
            nrOfCoinsCollected = value;
            UpdateCoinsText();
        }
    }

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

    private void UpdateCoinsText()
    {
        if (coinsText != null)
        {
            coinsText.text = "x " + NrOfCoinsCollected.ToString();
        }
    }

    public void StartPlaying()
    {
        playing = true;
        Score = 0L;
        NrOfCoinsCollected = 0;
        StartCoroutine(IncrementEveryHalfSecond());
    }

    /// <summary>
    /// Stops keeping score and updates the high score if a new high score was achieved.
    /// </summary>
    /// <returns>The current high score</returns>
    public long StopPlaying()
    {
        playing = false;
        var totalScore = Score * NrOfCoinsCollected;
        return UpdateHighScore(totalScore);
    }

    private long UpdateHighScore(long totalScore)
    {
        if (PlayerPrefs.HasKey(HighScoreKey))
        {
            if (long.TryParse(PlayerPrefs.GetString(HighScoreKey), out long loadedScore))
            {
                if(totalScore > loadedScore)
                {
                    PlayerPrefs.SetString(HighScoreKey, totalScore.ToString());
                    PlayerPrefs.Save();
                }
                return loadedScore;
            }
        }
        else
        {
            PlayerPrefs.SetString(HighScoreKey, totalScore.ToString());
            PlayerPrefs.Save();
        }
        return 0L;
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
