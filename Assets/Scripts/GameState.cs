using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    private State state = State.Intro;
    private GameInput gameInput;
    private DepthOfField gameOverBlur;

    public GameObject cylinder;
    public GameObject player;
    public GameObject barrierSpawnerSystem;
    public GameObject spawnerCoordinator;
    public GameObject coinSpawner;
    public GameObject gameCam;
    public GameObject startScreenCam;
    public GameObject hud;
    public GameObject gameOverCanvas;
    public GameObject scoreKeeper;
    public Volume blurVolume;

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
        StartCoroutine(ShowGameOverBlur());
    }

    private IEnumerator ShowGameOverBlur()
    {
        blurVolume.gameObject.SetActive(true);
        var maxBlur = 300f;
        var noBlur = 120f;
        var durationInSeconds = 1;
        for (var timePassed = 0f; timePassed < durationInSeconds; timePassed += Time.deltaTime)
        {
            var factor = timePassed / durationInSeconds;
            gameOverBlur.focalLength.Override(Mathf.Lerp(noBlur, maxBlur, factor));
            yield return null;
        }
    }

    private void GameState_OnTitleDismissed()
    {
        // this is when the game really starts
        this.startScreenCam.SetActive(false);
        this.gameCam.SetActive(true);
        spawnerCoordinator.SetActive(true);
        barrierSpawnerSystem.SetActive(true);
        coinSpawner.SetActive(true);
        cylinder.GetComponent<Cylinder>().StartAccelerating();
        scoreKeeper.GetComponent<ScoreKeeper>().StartPlaying();
        hud.SetActive(true);
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
        state = State.Playing;
        player.SetActive(true); // Activate to prevent pop-in of player
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
