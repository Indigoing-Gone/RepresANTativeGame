using System;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Jumping Variables")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float timeToJumpApex;
    [SerializeField] private float maxFallSpeed;

    [SerializeField] private float upwardsMult;
    [SerializeField] private float downwardsMult;
    [SerializeField] private float releaseMult;

    [Header("Assist Variables")]
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    [Header("Calculations")]
    private Vector2 desiredVelocity;
    private float verticalVelocity;

    private float jumpSpeed;
    private float defaultGravityMult;
    private float currentGravityMult;

    [Header("States")]
    private bool onGround;
    private bool pressingJump;
    private bool currentlyJumping;
    private bool jumpDesired;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityMult = 1f;
    }

    private void Update()
    {
        SetNewGravity();

        //Allows the player to input jump before they hit the ground and have it still be registered for a period of time
        if (jumpBuffer > 0 && jumpDesired)
        {
            jumpBufferCounter += Time.deltaTime;

            if (jumpBufferCounter > jumpBuffer)
            {
                jumpDesired = false;
                jumpBufferCounter = 0;
            }
        }

        //Runs coyote jump timer
        if (!currentlyJumping && !onGround) coyoteTimeCounter += Time.deltaTime;
        else coyoteTimeCounter = 0;
    }

    private void FixedUpdate()
    {
        desiredVelocity = rb.linearVelocity;
        verticalVelocity = Vector2.Dot(rb.linearVelocity, transform.up);

        if (jumpDesired) TriggerJump();
        else CalculateGravity();

        rb.linearVelocity = desiredVelocity;
    }

    private void CalculateGravity()
    {
        //Set the gravity multiplier based on various factors (on ground, moving up, moving down)
        if (onGround)
        {
            currentGravityMult = defaultGravityMult;
            if (Mathf.Abs(verticalVelocity) <= 0.01f) currentlyJumping = false;
        }
        else if (verticalVelocity > 0.01f) currentGravityMult = (pressingJump && currentlyJumping) ? upwardsMult : releaseMult;
        else if (verticalVelocity < -0.01f) currentGravityMult = downwardsMult;
        else currentGravityMult = defaultGravityMult;

        //Limit fall speed
        if (-verticalVelocity > maxFallSpeed)
        {
            desiredVelocity -= (Vector2)transform.up * verticalVelocity;
            desiredVelocity += (Vector2)transform.up * -maxFallSpeed;
        }

        rb.linearVelocity = desiredVelocity;
    }

    private void TriggerJump()
    {
        if (onGround || (coyoteTimeCounter > 0.03f && coyoteTimeCounter < coyoteTime))
        {
            //Reset assist variables for next jump
            jumpDesired = false;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;

            //Reset gravity scale to ensure proper jumpSpeed calculation
            currentGravityMult = defaultGravityMult;
            SetNewGravity();

            //Calculate desired jump velocity
            jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * rb.gravityScale * jumpHeight);
            desiredVelocity -= (Vector2)transform.up * verticalVelocity;
            desiredVelocity += (Vector2)transform.up * jumpSpeed;

            //Start jumping
            currentlyJumping = true;
        }

        if (jumpBuffer == 0) jumpDesired = false;
    }

    private void SetNewGravity()
    {
        //Set gravity scale based on jump parameters and gravity multiplier
        Vector2 newGravity = new(0, (-2 * jumpHeight) / (timeToJumpApex * timeToJumpApex));
        rb.gravityScale = (newGravity.y / Physics2D.gravity.y) * currentGravityMult;
    }

    public void ProcessInput(bool _state)
    {
        if (!pressingJump && _state) jumpDesired = true;
        pressingJump = _state;
    }

    public void ProcessGround(bool _state) => onGround = _state;
}
