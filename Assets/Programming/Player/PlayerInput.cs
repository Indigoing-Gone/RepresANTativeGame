using UnityEngine;

[CreateAssetMenu(menuName = "Input/PlayerInput")]
public class PlayerInput : ScriptableObject
{
    [SerializeField] private InputReader input;

    private Vector2 moveDirection;
    private bool pressingJump;

    public Vector2 MoveDirection { get => moveDirection; }
    public bool PressingJump { get => pressingJump; }

    private void OnEnable()
    {
        input.MoveEvent += HandleMove;
        input.JumpEvent += HandleJump;
    }

    private void OnDisable()
    {
        input.MoveEvent -= HandleMove;
        input.JumpEvent -= HandleJump;
    }

    private void HandleMove(Vector2 _direction) => moveDirection = _direction;
    private void HandleJump(bool _state) => pressingJump = _state;
}
