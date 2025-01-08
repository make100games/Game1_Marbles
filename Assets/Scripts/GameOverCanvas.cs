using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameOverCanvas : MonoBehaviour
{
    public TMP_Text pressAnyText;
    public float fadeDuration = 1f;
    public bool loopBlinking = true;

    private void Start()
    {
        if (pressAnyText == null)
        {
            Debug.LogError("TextMeshPro component is not assigned!");
            return;
        }

        // Start the blinking effect
        StartBlinking();
    }

    private void StartBlinking()
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
    }
}
