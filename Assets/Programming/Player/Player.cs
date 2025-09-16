using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerJump), typeof(PlayerGround))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerJump playerJump;
    private PlayerGround playerGround;
    private Animator animator;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        playerGround = GetComponent<PlayerGround>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Feed Player Movement input and ground
        playerMovement.ProcessInput(playerInput.MoveDirection.x);
        playerMovement.ProcessGround(playerGround.OnGround);

        //Feed Player Jump input and ground
        playerJump.ProcessInput(playerInput.PressingJump);
        playerJump.ProcessGround(playerGround.OnGround);

        //Animator BS
        if (playerInput.MoveDirection.x != 0) transform.localScale = new Vector2(Mathf.Sign(-playerInput.MoveDirection.x), 1);
        animator.SetFloat("MoveSpeed", Mathf.Abs(playerInput.MoveDirection.x));
        animator.SetBool("IsGrounded", playerGround.OnGround);
    }

    private void FixedUpdate()
    {

    }
}
