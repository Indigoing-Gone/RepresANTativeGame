using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerJump), typeof(PlayerGround))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private PlayerJump playerJump;
    private PlayerGround playerGround;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        playerGround = GetComponent<PlayerGround>();
    }

    private void Update()
    {
        //Feed Player Movement input and ground
        playerMovement.ProcessInput(playerInput.MoveDirection.x);
        playerMovement.ProcessGround(playerGround.OnGround);

        //Feed Player Jump input and ground
        playerJump.ProcessInput(playerInput.PressingJump);
        playerJump.ProcessGround(playerGround.OnGround);
    }

    private void FixedUpdate()
    {

    }
}
