using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class GameTitle : MonoBehaviour
{
    public float timeAfterWhichToShowTitle = 4f;
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1.5f;
    public GameObject gameTitleCanvas;
    public Image titleText;
    public TMP_Text pressAnyKeyText;
    public float fadeDuration = 1f;

    public event Action OnTitleAppeared;
    public event Action OnTitleDismissed;

    private Sequence blinkSequence;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowTitle", timeAfterWhichToShowTitle);
    }

    void ShowTitle()
    {
        titleText.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        Invoke("TitleFullyShown", (fadeInDuration * 2));  // Sadly no callback for when the tween is done. Pretty ugly but hey, whatcha gonna do
    }

    void TitleFullyShown()
    {
        OnTitleAppeared?.Invoke();
        StartFlashingPressAnyText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Folds title downwards and moves it out of view past the camera
    /// </summary>
    public void Dismiss()
    {
        if(blinkSequence != null)
        {
            blinkSequence.Pause();
            Color invisibleColor = pressAnyKeyText.color;
            invisibleColor.a = 0f;
            pressAnyKeyText.color = invisibleColor;
        }
        titleText.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        Invoke("TitleHidden", (fadeOutDuration) * 2);
    }

    void TitleHidden()
    {
        OnTitleDismissed?.Invoke();
    }

    private void StartFlashingPressAnyText()
    {
        // Ensure the alpha starts at 0
        Color initialColor = pressAnyKeyText.color;
        initialColor.a = 0f;
        pressAnyKeyText.color = initialColor;

        // Create the fade-in and fade-out sequence
        blinkSequence = DOTween.Sequence();

        // Fade-in animation
        blinkSequence.Append(pressAnyKeyText.DOFade(1f, fadeDuration));

        // Fade-out animation
        blinkSequence.Append(pressAnyKeyText.DOFade(0f, fadeDuration));

        // Loop the sequence indefinitely if required
        blinkSequence.SetLoops(-1, LoopType.Yoyo);

        // Play the sequence
        blinkSequence.Play();
    }
}
