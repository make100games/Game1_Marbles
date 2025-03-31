using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class GameTitle : MonoBehaviour
{
    public float timeAfterWhichToShowTitle = 4f;
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1.5f;
    public GameObject gameTitleCanvas;
    public Image titleText;

    public event Action OnTitleAppeared;
    public event Action OnTitleDismissed;

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
        titleText.DOFade(0f, fadeOutDuration).SetEase(Ease.InOutQuad);
        Invoke("TitleHidden", (fadeOutDuration) * 2);
    }

    void TitleHidden()
    {
        OnTitleDismissed?.Invoke();
    }
}
