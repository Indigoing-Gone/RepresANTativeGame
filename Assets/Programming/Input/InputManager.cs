using UnityEngine;

enum InputState
{
    None,
    Movement,
    Dialogue
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputState inputState;
    [SerializeField] private InputReader input;

    private void Awake()
    {
        input.DisableAll();
        input.SetMovement();

        inputState = InputState.None;
        ChangeInput(InputState.Movement);

        DialogueController.ConversationStarted += OnConversationStarted;
        DialogueController.ConversationEnded += OnConversationEnded;
    }

    private void OnConversationStarted() => ChangeInput(InputState.Dialogue);
    private void OnConversationEnded() => ChangeInput(InputState.Movement);

    private void ChangeInput(InputState _state)
    {
        if (inputState == _state) return;
        inputState = _state;

        if (inputState == InputState.Movement) input.SetMovement();
        else if (inputState == InputState.Dialogue) input.SetDialogue();
    }
}
