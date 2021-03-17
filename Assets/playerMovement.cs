using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class playerMovement : MonoBehaviour
{
    public Vector2 speed = new Vector2(50, 50);
    public DashState dashState;
    public float moveSpeed;
    public Rigidbody2D rb;
    private Vector2 playerInput;
    public float jumpSpeed;
    public bool shouldJump;
    private SpriteRenderer spriteR;
    public float dashForce;
    public float dashTimer = 0;
    public float maxDash = 20f;
    private float characterDirection;
    public Vector2 savedVelocity;
    // Start is called before the first frame update
    [SerializeField]
    private float raySize;
    [SerializeField]
    private Color rayColor;
    [SerializeField]
    private PlayerState playerState;

    private PlayerCollisionHelper playerCollisionHelper;

    private void Awake()
    {
        playerCollisionHelper = new PlayerCollisionHelper();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        spriteR.flipX = true;
        characterDirection = 1f;
        dashState = DashState.READY;
        playerState = PlayerState.GROUNDED;
    }

    // Update is called once per frame
    void Update()
    {
        playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (playerState == PlayerState.GROUNDED && Input.GetKeyDown(KeyCode.Space))
        {
            shouldJump = true;
        }
        dashing();
        if (!playerCollisionHelper.IsPLayerGrounded(rb) && playerState != PlayerState.WALL_GRINDING)
        {
            playerState = PlayerState.JUMPING;
        }
        else if(playerState != PlayerState.WALL_GRINDING)
        {
            playerState = PlayerState.GROUNDED;
        }
    }

    void FixedUpdate()
    {
        // UnityEngine.Debug.Log(playerInput.y);

        if (playerInput != Vector2.zero && dashState != DashState.DASHING)
        {
            rb.velocity = new Vector3(playerInput.x * moveSpeed, rb.velocity.y, 0);
        }
        if (shouldJump)
        {
            rb.AddRelativeForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            shouldJump = false;
        }
        if (playerState == PlayerState.JUMPING)
        {
            Vector3 vel = rb.velocity;
            vel.y -= 1 * Time.deltaTime;
            rb.velocity = vel;
        }

        if (playerCollisionHelper.WallInFrontOfMe(rb, characterDirection))
        {
            if (dashState == DashState.DASHING)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                dashTimer = maxDash;
                savedVelocity = new Vector2(0, 0);
            }
            if (playerState == PlayerState.JUMPING || playerState == PlayerState.WALL_GRINDING)
            {
                if (characterDirection > 0 && playerInput.x > 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, 0);

                    playerState = PlayerState.WALL_GRINDING;
                }
                else if(characterDirection < 0 && playerInput.x < 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, 0);
                    playerState = PlayerState.WALL_GRINDING;
                }
                else
                {
                    rb.gravityScale = 1;
                }
            }
        }
        else if(playerState == PlayerState.WALL_GRINDING)
        {
            rb.gravityScale = 1;
            if (!playerCollisionHelper.IsPLayerGrounded(rb))
            {
                playerState = PlayerState.JUMPING;
            }
            else
            {
                playerState = PlayerState.GROUNDED;
            }
        }

        FlipPlayerSprite();
    }

    private void FlipPlayerSprite()
    {
        if (rb.velocity.x > 0.01f && !spriteR.flipX)
        {
            spriteR.flipX = true;
            characterDirection = 1f;
        }
        if (rb.velocity.x < -0.01f && spriteR.flipX)
        {
            spriteR.flipX = false;
            characterDirection = -1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 direction = transform.TransformDirection(new Vector3(this.characterDirection, 0)) * raySize;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.DrawRay(transform.position, Vector2.down);
    }

    private void dashing()
    {
        switch (dashState)
        {
            case DashState.READY:
                var isDashKeyDown = Input.GetKeyDown(KeyCode.LeftShift);
                if (isDashKeyDown)
                {
                    savedVelocity = new Vector2(rb.velocity.x, 0);
                    float dashVelocity;
                    if (spriteR.flipX)
                    {
                        dashVelocity = 1f;
                    }
                    else
                    {
                        dashVelocity = -1f;
                    }
                    rb.velocity = new Vector2(dashVelocity * dashForce, 0);
                    dashState = DashState.DASHING;
                    rb.isKinematic = true;
                }
                break;
            case DashState.DASHING:
                dashTimer += Time.deltaTime * 3;
                if (dashTimer >= maxDash)
                {
                    rb.isKinematic = false;
                    dashTimer = maxDash;
                    rb.velocity = savedVelocity;
                    dashState = DashState.COOLDOWN;
                }
                break;
            case DashState.COOLDOWN:
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0)
                {
                    dashTimer = 0;
                    dashState = DashState.READY;
                }
                break;
        }
    }
}
