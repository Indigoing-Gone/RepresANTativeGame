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
    [SerializeField] private GameState inputState;
    [SerializeField] private InputReader input;
    [SerializeField] private CinemachineCamera towerCamera;

    private void Awake()
    {
        input.DisableAll();
        input.SetMovement();

        inputState = GameState.None;
        ChangeInput(GameState.Movement);

        DialogueController.ConversationStarted += OnConversationStarted;
        DialogueController.ConversationEnded += OnConversationEnded;

        TowerManager.StartingTower += OnStartingTower;
        TowerManager.EndingTower += OnEndingTower;
    }

    private void OnConversationStarted() => ChangeInput(GameState.Dialogue);
    private void OnConversationEnded() => ChangeInput(GameState.Movement);

    private void OnStartingTower()
    {
        ChangeInput(GameState.Tower);
        towerCamera.Priority = 2;
    }

    private void OnEndingTower()
    {
        ChangeInput(GameState.Movement);
        towerCamera.Priority = 0;
    }

    private void ChangeInput(GameState _state)
    {
        if (inputState == _state) return;
        inputState = _state;

        if (inputState == GameState.Movement) input.SetMovement();
        if (inputState == GameState.Dialogue) input.SetDialogue();
        if (inputState == GameState.Tower) input.SetTower();
    }
}
