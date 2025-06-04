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
    public Image buttonPanelImage;
    public Image startButtonImage;
    public Text startButtonText;
    public Image controlsButtonImage;
    public Text controlsButtonText;
    public Image creditsButtonImage;
    public Text creditsButtonText;
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
        buttonPanelImage.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        startButtonImage.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        startButtonText.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        controlsButtonImage.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        controlsButtonText.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        creditsButtonImage.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
        creditsButtonText.DOFade(1f, fadeInDuration).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        buttonPanelImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        startButtonImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        startButtonText.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        controlsButtonImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        controlsButtonText.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        creditsButtonImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        creditsButtonText.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        Invoke("TitleHidden", (fadeOutDuration) * 2);
    }

    void TitleHidden()
    {
        OnTitleDismissed?.Invoke();
    }
}
