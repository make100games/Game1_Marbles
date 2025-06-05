using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GameOverCanvas : MonoBehaviour
{
    public Image panelBackground;
    public TMP_Text gameOverText;
    public Text scoreText;
    public TMP_Text pressAnyText;
    public float fadeDuration = 1f;
    public bool loopBlinking = true;

    public event Action onReadyToPlayAgain;

    public void ShowScore(long score, int nrCoins, long highScore)
    {
        var totalScore = score * nrCoins;
        if(totalScore > highScore)
        {
            scoreText.text = "New High Score!\n" +
                "Score: " + score + "\n" +
                "Coins: " + nrCoins + "\n" +
                "Total score (" + score + " x " + nrCoins +"): " + totalScore + "\n";
        }
        else
        {
            scoreText.text = "Score: " + score + "\n" +
                "Coins: " + nrCoins + "\n" +
                "Total score (" + score + " x " + nrCoins + "): " + totalScore + "\n" +
                "High score: " + highScore;
        }
    }

    private void Start()
    {
        if (gameOverText == null || scoreText == null || pressAnyText == null)
        {
            Debug.LogError("TextMeshPro components are not assigned!");
            return;
        }

        // Set alpha of texts to 0 initially
        Color initialGameOverTextColor = gameOverText.color;
        initialGameOverTextColor.a = 0f;
        gameOverText.color = initialGameOverTextColor;

        Color initialScoreTextColor = scoreText.color;
        initialScoreTextColor.a = 0f;
        scoreText.color = initialScoreTextColor;

        Color initialPressAnyTextColor = pressAnyText.color;
        initialPressAnyTextColor.a = 0f;
        pressAnyText.color = initialPressAnyTextColor;

        Invoke("FadeInGameOverText", 1f);

        Invoke("FadeInScoreText", 2f);

        Invoke("StartFlashingPressAnyText", 5f);
    }

    private void FadeInGameOverText()
    {
        gameOverText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
    }

    private void FadeInScoreText()
    {
        scoreText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        panelBackground.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
    }

    private void StartFlashingPressAnyText()
    {
        // Ensure the alpha starts at 0
        Color initialColor = pressAnyText.color;
        initialColor.a = 0f;
        pressAnyText.color = initialColor;

        // Create the fade-in and fade-out sequence
        Sequence blinkSequence = DOTween.Sequence();

        // Fade-in animation
        blinkSequence.Append(pressAnyText.DOFade(1f, fadeDuration));

        // Fade-out animation
        blinkSequence.Append(pressAnyText.DOFade(0f, fadeDuration));

        // Loop the sequence indefinitely if required
        if (loopBlinking)
        {
            blinkSequence.SetLoops(-1, LoopType.Yoyo);
        }

        // Play the sequence
        blinkSequence.Play();

        onReadyToPlayAgain?.Invoke();
    }
}
