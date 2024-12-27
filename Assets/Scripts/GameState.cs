using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private State state = State.StartScreen;
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
    }

    private void StartGame_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        switch (state)
        {
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
        cylinder.GetComponent<Cylinder>().StartAccelerating();
        player.SetActive(true);
        spawnerCoordinator.SetActive(true);
        coinSpawner.SetActive(true);
        gameCam.SetActive(true);
        startScreenCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum State
    {
        StartScreen,
        Playing,
        HighScoreScreen
    }
}
