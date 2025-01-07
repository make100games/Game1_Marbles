using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private State state = State.Intro;
    private GameInput gameInput;

    public GameObject cylinder;
    public GameObject player;
    public GameObject spawnerCoordinator;
    public GameObject coinSpawner;
    public GameObject gameCam;
    public GameObject startScreenCam;

    // Start is called before the first frame update
    void Start()
    {
        gameInput = new GameInput();
        gameInput.Enable();
        gameInput.Game.StartGame.performed += StartGame_performed;
        startScreenCam.GetComponent<StartScreenCam>().OnTitleFullyDisplayed += GameState_OnTitleFullyDisplayed;
        startScreenCam.GetComponent<StartScreenCam>().OnTitleDismissed += GameState_OnTitleDismissed;
        player.GetComponent<Player>().OnPlayerLost += GameState_OnPlayerLost;
    }

    private void GameState_OnPlayerLost()
    {
        state = State.HighScoreScreen;
    }

    private void GameState_OnTitleDismissed()
    {
        // this is when the game really starts
        this.startScreenCam.SetActive(false);
        this.gameCam.SetActive(true);
        spawnerCoordinator.SetActive(true);
        coinSpawner.SetActive(true);
        cylinder.GetComponent<Cylinder>().StartAccelerating();
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
        HighScoreScreen
    }
}
