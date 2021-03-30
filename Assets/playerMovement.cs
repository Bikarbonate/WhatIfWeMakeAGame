using Assets;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public Vector2 speed = new Vector2(50, 50);
    public Rigidbody2D rb;
    private BoxCollider2D bc;
    private Vector2 playerInput;
    public bool startJump;
    private SpriteRenderer spriteR;
    public Vector2 savedVelocity;
    private bool shouldWallJump = false;
    // Start is called before the first frame update
    [SerializeField]
    private float raySize;
    [SerializeField]
    private Color rayColor;
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerDto dto;
    [SerializeField]
    private float maxFallingSpeed;
    private bool shouldKeepJumping;

    private PlayerCollisionHelper playerCollisionHelper;

    private void Awake()
    {
        playerCollisionHelper = new PlayerCollisionHelper();
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        player = new Player(rb, playerCollisionHelper, spriteR, bc, dto);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        movementStateLogic();
        dashStateLogic();
    }

    void FixedUpdate()
    {
        if (player.playerState == PlayerState.DASHING)
        {
            return;
        }
        if (playerInput != Vector2.zero && player.playerState == PlayerState.GROUNDED)
        {
            player.MovePlayer(playerInput);
        }
        else if (playerInput != Vector2.zero && (player.playerState == PlayerState.FALLING
            || player.playerState == PlayerState.JUMPING))
        {
            player.MovePlayerWhileJumping(playerInput);
        }
        if (playerInput == Vector2.zero
            && player.playerState == PlayerState.GROUNDED)
        {
            player.StopPlayer();
        }
        if (startJump || shouldKeepJumping)
        {
            if (!player.Jump(shouldKeepJumping))
            {
                startJump = false;
                shouldKeepJumping = false;
            }

        }
        if (shouldWallJump)
        {
            player.StartWallJump(playerInput);
            shouldWallJump = false;
        }
        if (player.playerState == PlayerState.WALL_JUMPING)
        {
            StartCoroutine(player.WallJumpRoutine());
        }
        if (player.playerState == PlayerState.FALLING)
        {
            player.Falling();
        }

        if (player.WallInFrontOfPlayer())
        {
            player.WallCollisionResolver(playerInput);
        }
        else if (player.playerState == PlayerState.WALL_GRINDING)
        {
            player.EndingWallGrind();
        }
        player.FlipPlayerSprite();
        player.CheckIfPLayerGrounded();
        if (player.rb.velocity.y > maxFallingSpeed)
            player.rb.velocity = new Vector2(player.rb.velocity.x, maxFallingSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(new Vector3(player.characterDirection, 0)) * raySize;
        Gizmos.DrawRay(transform.position, Vector2.down);
    }

    private void dashStateLogic()
    {
        if (player.playerState != PlayerState.WALL_JUMPING)
        {
            switch (player.dashState)
            {
                case State.READY:
                    if (Input.GetButtonDown("Dash"))
                        player.StartDash();
                    break;
                case State.DASHING:
                    player.Dashing();
                    break;
                case State.COOLDOWN:
                    player.DashCooldown();
                    if (player.playerState == PlayerState.JUMPING)
                    {
                        shouldKeepJumping = false;
                    }
                    break;
            }
        }
    }
    private void movementStateLogic()
    {
        switch (player.playerState)
        {
            case PlayerState.GROUNDED:
                if (Input.GetButtonDown("Jump"))
                {
                    startJump = true;
                    player.playerState = PlayerState.JUMPING;
                }
                break;
            case PlayerState.JUMPING:
                if (Input.GetButton("Jump"))
                    shouldKeepJumping = true;
                if (Input.GetButtonUp("Jump"))
                {
                    shouldKeepJumping = false;
                    shouldWallJump = false;
                }
                break;
            case PlayerState.FALLING:

                if (Input.GetButton("Jump"))
                {
                    startJump = false;
                    shouldKeepJumping = false;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    shouldKeepJumping = false;
                    shouldWallJump = false;
                }
                break;
            case PlayerState.WALL_GRINDING:
            case PlayerState.WALL_SLIDING:
                if (Input.GetButtonDown("Jump"))
                {
                    shouldWallJump = true;
                    player.playerState = PlayerState.JUMPING;
                }
                break;
            case PlayerState.WALL_JUMPING:
                break;
            default:
                break;
        }
    }
}
