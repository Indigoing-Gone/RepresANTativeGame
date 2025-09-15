using UnityEngine;

[CreateAssetMenu(menuName = "Input/TowerInput")]
public class TowerInput : MonoBehaviour
{
    [SerializeField] private InputReader input;

    [SerializeField] private Vector2 pointerPosition;
    [SerializeField] private bool pressingDrag;
    [SerializeField] private bool pressingRotate;

    public Vector2 PointerPosition { get => pointerPosition; }
    public bool PressingDrag { get => pressingDrag; }
    public bool PressingRotate { get => pressingRotate; }

    private void OnEnable()
    {
        input.PositionEvent += HandlePosition;
        input.DragEvent += HandleDrag;
        input.RotateEvent += HandleRotate;
    }

    private void OnDisable()
    {
        input.PositionEvent -= HandlePosition;
        input.DragEvent -= HandleDrag;
        input.RotateEvent -= HandleRotate;
    }

    private void HandlePosition(Vector2 _position) => pointerPosition = _position;
    private void HandleDrag(bool _state) => pressingDrag = _state;
    private void HandleRotate(bool _state) => pressingRotate = _state;
}
