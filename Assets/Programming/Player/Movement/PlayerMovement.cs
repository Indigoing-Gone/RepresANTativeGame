using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Movement Variables")]
    [SerializeField] private float maxSpeed;

    [SerializeField] private float maxAcceleration;
    [SerializeField] private float maxDeceleration;
    [SerializeField] private float maxTurnSpeed;

    [SerializeField] private float maxAirAcceleration;
    [SerializeField] private float maxAirDeceleration;
    [SerializeField] private float maxAirTurnSpeed;

    [SerializeField] private float friction;

    [Header("Calculations")]
    private Vector2 desiredVelocity;
    private float maxSpeedChange;

    private float acceleration;
    private float deceleration;
    private float turnSpeed;

    [Header("States")]
    private bool onGround;
    private bool pressingKey;
    private bool isTurning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        //Set acceleration values depending if the player is grounded
        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDeceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

        //Get the maximum amount the speed can change by on the current time step
        if (pressingKey) maxSpeedChange = (isTurning ? turnSpeed : acceleration) * Time.fixedDeltaTime;
        else maxSpeedChange = deceleration * Time.fixedDeltaTime;

        //Calculate and apply new speed to the player
        Vector2 currentHorizontal = Vector3.Project(rb.linearVelocity, transform.right);
        Vector2 targetHorizontal = transform.right * desiredVelocity.x;
        Vector2 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetHorizontal, maxSpeedChange);
        Vector2 finalVelocity = rb.linearVelocity - currentHorizontal + newHorizontal;

        rb.linearVelocity = finalVelocity;
    }

    public void ProcessInput(float _direction)
    {
        desiredVelocity = new Vector2(_direction, 0f) * Mathf.Max(maxSpeed - friction, 0f);
        pressingKey = _direction != 0;
        isTurning = Mathf.Sign(_direction) != Mathf.Sign(rb.linearVelocity.x);
    }

    public void ProcessGround(bool _state) => onGround = _state;
}
