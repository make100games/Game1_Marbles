using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameTitle : MonoBehaviour
{
    public float timeAfterWhichToShowTitle = 4f;
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1.5f;
    public GameObject gameTitleCanvas;
    public GameObject controlsCanvas;
    public GameObject creditsCanvas;
    public Image titleText;
    public TMP_Text pressAnyKeyText;
    public Image buttonPanelImage;
    public Image startButtonImage;
    public Text startButtonText;
    public Image controlsButtonImage;
    public Text controlsButtonText;
    public Image creditsButtonImage;
    public Text creditsButtonText;
    public Image controlsBackground;
    public Image controlsTitle;
    public Text controlsTitleText;
    public Text controlsText;
    public Image controlsToMainButtonImage;
    public Text controlsToMainButtonText;
    public Image creditsBackground;
    public Image creditsTitle;
    public Text creditsTitleText;
    public Text creditsText;
    public Image creditsToMainButtonImage;
    public Text creditsToMainButtonText;
    public float fadeDuration = 1f;
    public Volume blurVolume;

    public event Action OnTitleAppeared;
    public event Action OnTitleDismissed;

    private Sequence blinkSequence;
    private DepthOfField gameOverBlur;

    // Start is called before the first frame update
    void Start()
    {
        gameOverBlur = Blur.ObtainBlur(blurVolume);
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

    public void ShowControlsMenu()
    {
        gameTitleCanvas.SetActive(false);
        controlsCanvas.SetActive(true);
        controlsBackground.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        controlsTitle.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        controlsTitleText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        controlsText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        controlsToMainButtonImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        controlsToMainButtonText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        StartCoroutine(Blur.ShowBlur(blurVolume, gameOverBlur));
    }

    public void ShowCreditsMenu()
    {
        gameTitleCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
        creditsBackground.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        creditsTitle.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        creditsTitleText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        creditsText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        creditsToMainButtonImage.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        creditsToMainButtonText.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad);
        StartCoroutine(Blur.ShowBlur(blurVolume, gameOverBlur));
    }

    public void HideControlsMenu()
    {
        gameTitleCanvas.SetActive(true);
        controlsCanvas.SetActive(false);
        controlsBackground.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        controlsTitle.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        controlsTitleText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        controlsText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        controlsToMainButtonImage.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        controlsToMainButtonText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        Blur.HideBlur(blurVolume);
    }

    public void HideCreditsMenu()
    {
        gameTitleCanvas.SetActive(true);
        creditsCanvas.SetActive(false);
        creditsBackground.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        creditsTitle.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        creditsTitleText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        creditsText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        creditsToMainButtonImage.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        creditsToMainButtonText.DOFade(0f, 0).SetEase(Ease.InOutQuad);
        Blur.HideBlur(blurVolume);
    }

    void TitleHidden()
    {
        OnTitleDismissed?.Invoke();
    }
}
