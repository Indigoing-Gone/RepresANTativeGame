using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[CreateAssetMenu(menuName = "Input/InputReader")]
[DefaultExecutionOrder(-1)]
public class InputReader : ScriptableObject, IMovementActions, IDialogueActions, ITowerActions
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
            playerInput.Tower.SetCallbacks(this);

            DisableAll();
        }
    }

    public void SetMovement()
    {
        playerInput.Movement.Enable();
        playerInput.Dialogue.Disable();
        playerInput.Tower.Disable();
    }

    public void SetDialogue()
    {
        playerInput.Movement.Disable();
        playerInput.Dialogue.Enable();
        playerInput.Tower.Disable();
    }

    public void SetTower()
    {
        playerInput.Movement.Disable();
        playerInput.Dialogue.Disable();
        playerInput.Tower.Enable();
    }

    public void DisableAll()
    {
        playerInput.Movement.Disable();
        playerInput.Dialogue.Disable();
        playerInput.Tower.Disable();
    }

    #endregion

    #region Events

    //Basic Movement
    public event Action<Vector2> MoveEvent;
    public event Action<bool> JumpEvent;
    public event Action<bool> InteractEvent;

    //Dialogue
    public event Action<bool> AdvanceEvent;

    //Tower
    public event Action<Vector2> PositionEvent;
    public event Action<bool> DragEvent;
    public event Action<bool> RotateEvent;

    #endregion

    #region Triggers

    //Horizontal Movement
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    //Jumping
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) JumpEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) JumpEvent?.Invoke(false);
    }

    //Interaction
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) InteractEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) InteractEvent?.Invoke(false);
    }

    //Dialogue Interaction
    public void OnAdvance(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) AdvanceEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) AdvanceEvent?.Invoke(false);
    }

    //Mouse Position
    public void OnPosition(InputAction.CallbackContext context)
    {
        PositionEvent?.Invoke(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()));
    }

    //Block Drag
    public void OnDrag(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) DragEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) DragEvent?.Invoke(false);
    }

    //Block Rotate
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) RotateEvent?.Invoke(true);
        if (context.phase == InputActionPhase.Canceled) RotateEvent?.Invoke(false);
    }

    #endregion
}
