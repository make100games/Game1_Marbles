using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameState : MonoBehaviour
{
    private State state = State.Intro;
    private GameInput gameInput;
    private DepthOfField gameOverBlur;

    public TMP_Text runningScoreText;
    public Image coinImage;
    public TMP_Text coinText;
    public GameObject cylinder;
    public GameObject player;
    public GameObject everythingSpawner;
    public GameObject barrierSpawnerSystem;
    public GameObject spawnerCoordinator;
    public GameObject coinSpawner;
    public GameObject detonator1;
    public GameObject detonator2;
    public GameObject gameCam;
    public GameObject startScreenCam;
    public GameObject hud;
    public GameObject gameOverCanvas;
    public GameObject scoreKeeper;
    public Volume blurVolume;
    public GameObject startSoundEffect;
    public float timeAfterWhichToStartSpawning = 1.5f;  // Time in seconds after which we start spawning basic obstacles. Coins will start spawning immediately.
    public float timeAfterWhichToStartSpawnerCoordinator = 3.5f;    // Time after which to start the waves of spawners

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        gameInput.Game.StartGame.performed += StartGame_performed;
        startScreenCam.GetComponent<StartScreenCam>().OnTitleFullyDisplayed += GameState_OnTitleFullyDisplayed;
        startScreenCam.GetComponent<StartScreenCam>().OnTitleDismissed += GameState_OnTitleDismissed;
        player.GetComponent<Player>().OnPlayerLost += GameState_OnPlayerLost;
        gameOverCanvas.GetComponent<GameOverCanvas>().onReadyToPlayAgain += GameState_onReadyToPlayAgain;

        DepthOfField temp;
        if (blurVolume.profile.TryGet<DepthOfField>(out temp))
        {
            gameOverBlur = temp;
        }
    }

    private void GameState_onReadyToPlayAgain()
    {
        state = State.HighScoreScreenReadyToPlayAgain;
    }

    private void GameState_OnPlayerLost()
    {
        hud.SetActive(false);
        var highScore = scoreKeeper.GetComponent<ScoreKeeper>().StopPlaying();
        state = State.HighScoreScreen;
        gameOverCanvas.SetActive(true);
        gameOverCanvas.GetComponent<GameOverCanvas>().
            ShowScore(scoreKeeper.GetComponent<ScoreKeeper>().Score, scoreKeeper.GetComponent<ScoreKeeper>().NrOfCoinsCollected, highScore);
        StartCoroutine(Blur.ShowBlur(blurVolume, gameOverBlur));
        spawnerCoordinator.GetComponent<SpawnerCoordinator>().StopSpawning();
        spawnerCoordinator.SetActive(false);
        everythingSpawner.SetActive(false);
        coinSpawner.SetActive(false);
    }

    private void GameState_OnTitleDismissed()
    {
        // this is when the game really starts
        runningScoreText.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
        coinImage.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
        coinText.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
        this.startScreenCam.SetActive(false);
        this.gameCam.SetActive(true);
        detonator1.SetActive(true);
        detonator2.SetActive(true);
        coinSpawner.SetActive(true);
        cylinder.GetComponent<Cylinder>().StartAccelerating();
        scoreKeeper.GetComponent<ScoreKeeper>().StartPlaying();
        hud.SetActive(true);
        Invoke("StartSpawningBasicObjects", timeAfterWhichToStartSpawning);
        Invoke("StartSpawningWavesOfObstacles", timeAfterWhichToStartSpawnerCoordinator);
    }

    private void StartSpawningBasicObjects()
    {
        everythingSpawner.SetActive(true);
    }

    private void StartSpawningWavesOfObstacles()
    {
        spawnerCoordinator.SetActive(true);
    }

    private void GameState_OnTitleFullyDisplayed()
    {
        state = State.StartScreen;
    }

    private void StartGame_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        switch (state)
        {
            case State.Intro:
                // No op
                break;
            case State.StartScreen:
                // No op
                break;
            case State.Playing:
                // No op
                break;
            case State.HighScoreScreen:
                // No op
                break;
            case State.HighScoreScreenReadyToPlayAgain:
                gameInput.Disable();
                // TODO skip the intro screen when restarting scene (e.g. by using global object)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

    public void StartButtonClicked()
    {
        StartPlaying();
    }

    private void StartGame()
    {
        switch (state)
        {
            case State.Intro:
                break;
            case State.StartScreen:
                StartPlaying();
                break;
            case State.Playing:
                // No op
                break;
            case State.HighScoreScreen:
                // No op
                break;
            case State.HighScoreScreenReadyToPlayAgain:
                gameInput.Disable();
                // TODO skip the intro screen when restarting scene (e.g. by using global object)
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

    private void StartPlaying()
    {
        startSoundEffect.GetComponent<AudioSource>().Play();
        state = State.Playing;
        player.SetActive(true); // Activate to prevent pop-in of player
        player.GetComponentInChildren<Plane>().StartPlayingThrusterSoundEffect();   // Must happen after we activate player!
        startScreenCam.GetComponent<StartScreenCam>().StartPlaying();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum State
    {
        Intro,  // While start screen is animating in
        StartScreen,
        Playing,
        HighScoreScreen,
        HighScoreScreenReadyToPlayAgain
    }
}
