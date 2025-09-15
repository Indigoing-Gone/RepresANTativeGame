using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(menuName = "Input/InputReader")]
[DefaultExecutionOrder(-1)]
public class InputReader : ScriptableObject, IMovementActions, IDialogueActions
{
    private PlayerInputActions playerInput;

    #region Callbacks

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInputActions();
            playerInput.Movement.SetCallbacks(this);
            playerInput.Dialogue.SetCallbacks(this);

            DisableAll();
        }
    }

    public void SetMovement()
    {
        playerInput.Movement.Enable();
        playerInput.Dialogue.Disable();
    }

    public void SetDialogue()
    {
        playerInput.Dialogue.Enable();
        playerInput.Movement.Disable();
    }

    public void DisableAll()
    {
        playerInput.Movement.Disable();
        playerInput.Dialogue.Disable();
    }

    #endregion

    #region Events

    //Basic Movement
    public event Action<Vector2> MoveEvent;
    public event Action<bool> JumpEvent;
    public event Action<bool> InteractEvent;

    //Dialogue
    public event Action<bool> AdvanceEvent;

    #endregion

    #region Triggers

    //Basic Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) JumpEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) JumpEvent?.Invoke(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) InteractEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) InteractEvent?.Invoke(false);
    }

    public void OnAdvance(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) AdvanceEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) AdvanceEvent?.Invoke(false);
    }

    #endregion
}
