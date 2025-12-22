using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

// Keeps tracks of all of the stages within the game. A State Machine is the more complex alternative for better scaling.
public class ExampleGameManager : Singleton<ExampleGameManager>
{

    [SerializeField] PlayerInputManager playerInputManager;
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;

    public GameState State { get; private set; }

    //private void Start() =>

    public void ChangeState(GameState newState)
    {
        //Debug.Log("New state: " + newState);
        //Debug.Log("Old state: " + State);
        if (State == newState) return;
        State = newState;
        OnBeforeStateChanged?.Invoke(newState);

        switch (newState) 
        {
            case GameState.Lobby:

                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.LoopingChase:

                break;
            case GameState.FinalCountDown:

                break;
            case GameState.PlayerDeath:

                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }

        OnAfterStateChanged?.Invoke(newState);

        //Debug.Log($"New state: {newState}");
    }

    private void HandleStarting()
    {
        // Start
    }

}

// An example of different gameState stages, organized via Enums.
[Serializable]
public enum GameState
{
    Lobby = 0,
    Starting = 1,
    LoopingChase = 2,
    FinalCountDown = 3,
    PlayerDeath = 4,
    Win = 5,
    Lose = 6,
}
