using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class playerMovement : MonoBehaviour
{
    public Vector2 speed = new Vector2(50, 50);
    public Rigidbody2D rb;
    private BoxCollider2D bc;
    private Vector2 playerInput;
    public bool shouldJump;
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
        //if (Input.GetAxis("Horizontal") >  0.01f && Input.GetAxis("Vertical") > 0.01f)
        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButton("Jump"))
        {
            if (player.playerState == PlayerState.GROUNDED)
            {
                shouldJump = true; 
                player.playerState = PlayerState.JUMPING;
            }
            if (player.playerState == PlayerState.JUMPING)
            {
               shouldKeepJumping = true;
            }
            if (player.playerState == PlayerState.WALL_GRINDING)
            {
                shouldWallJump = true;
                player.playerState = PlayerState.JUMPING;
            }
            if (player.playerState == PlayerState.FALLING)
            {
                shouldJump = false;
                shouldKeepJumping = false;
            }
        }
     
        if (Input.GetButtonUp("Jump"))
        {
            if (player.playerState == PlayerState.JUMPING || player.playerState == PlayerState.FALLING)
            {
                shouldKeepJumping = false;
                shouldWallJump = false;
            }
        }
        dashing();
    }

    void FixedUpdate()
    {
        // UnityEngine.Debug.Log(playerInput.y);

        if (playerInput != Vector2.zero && player.dashState != State.DASHING 
            && player.playerState == PlayerState.GROUNDED)
        {
            player.MovePlayer(playerInput);
        }
        else if (playerInput != Vector2.zero && (player.playerState != PlayerState.FALLING
            || player.playerState != PlayerState.JUMPING)
            && player.dashState != State.DASHING)
        {
            player.MovePlayerWhileJumping(playerInput);
        }
        if (shouldJump || shouldKeepJumping)
        {
            if(!player.Jump(shouldKeepJumping))
            {
                shouldJump = false;
            }
        }
        if(shouldWallJump || player.playerState == PlayerState.WALL_JUMPING)
        {
            player.WallJump();
            shouldWallJump = false;
            shouldJump = true;
        }
        if (player.playerState == PlayerState.FALLING)
        {
            player.Falling();
        }

        if (player.WallInFrontOfPlayer())
        {
            player.WallCollisionResolver(playerInput);
        }
        else if(player.playerState == PlayerState.WALL_GRINDING)
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

    private void dashing()
    {
        switch (player.dashState)
        {
            case State.READY:
                if (Input.GetButtonDown("Dash"))
                    player.TryDash();
                break;
            case State.DASHING:
                player.Dashing();
                break;
            case State.COOLDOWN:
                player.DashCooldown();
                break;
        }
    }
}
