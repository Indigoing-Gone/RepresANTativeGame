using System;
using Unity.Cinemachine;
using UnityEngine;

enum GameState
{
    None,
    Movement,
    Dialogue,
    Tower
}

public class StateManager : MonoBehaviour
{
    [SerializeField] private GameState currentState;
    [SerializeField] private GameState returnState;
    [SerializeField] private InputReader input;
    [SerializeField] private CinemachineCamera towerCamera;

    public static event Action TowerStarted;

    private void Awake()
    {
        input.DisableAll();
        input.SetMovement();

        currentState = GameState.None;
        ChangeInput(GameState.Movement);

        DialogueController.ConversationStarted += OnConversationStarted;
        DialogueController.ConversationEnded += OnConversationEnded;

        TowerManager.StartingTower += OnStartingTower;
        TowerManager.EndingTower += OnEndingTower;
    }

    private void OnConversationStarted()
    {
        returnState = currentState;
        ChangeInput(GameState.Dialogue);
    }

    private void OnConversationEnded() => ChangeInput(returnState);

    private void OnStartingTower()
    {
        ChangeInput(GameState.Tower);
        towerCamera.Priority = 2;
        TowerStarted?.Invoke();
    }

    private void OnEndingTower()
    {
        ChangeInput(GameState.Movement);
        towerCamera.Priority = 0;
    }

    private void ChangeInput(GameState _state)
    {
        if (currentState == _state) return;
        currentState = _state;

        if (currentState == GameState.Movement) input.SetMovement();
        if (currentState == GameState.Dialogue) input.SetDialogue();
        if (currentState == GameState.Tower) input.SetTower();
    }
}
