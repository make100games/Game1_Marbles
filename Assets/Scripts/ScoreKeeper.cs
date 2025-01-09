using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ScoreKeeper : MonoBehaviour
{
    private long score;
    private bool playing = false;
    private const string HighScoreKey = "HighScore";

    public TMP_Text runningScoreText;
    public long Score
    {
        get => score;
        private set
        {
            score = value;
            UpdateScoreText();
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

    public void StartPlaying()
    {
        playing = true;
        Score = 0L;
        StartCoroutine(IncrementEveryHalfSecond());
    }

    /// <summary>
    /// Stops keeping score and updates the high score if a new high score was achieved.
    /// </summary>
    /// <returns>The current high score</returns>
    public long StopPlaying()
    {
        playing = false;
        return UpdateHighScore();
    }

    private long UpdateHighScore()
    {
        if (PlayerPrefs.HasKey(HighScoreKey))
        {
            if (long.TryParse(PlayerPrefs.GetString(HighScoreKey), out long loadedScore))
            {
                if(Score > loadedScore)
                {
                    PlayerPrefs.SetString(HighScoreKey, Score.ToString());
                    PlayerPrefs.Save();
                }
                return loadedScore;
            }
        }
        else
        {
            PlayerPrefs.SetString(HighScoreKey, Score.ToString());
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
