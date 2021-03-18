using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class Player
    {
        public Rigidbody2D rb;
        private SpriteRenderer playerSprite;
        private PlayerCollisionHelper playerCollisionHelper;
        private Vector2 dashSavedVelocity;
        private BoxCollider2D pc;

        [SerializeField]
        public DashState dashState;
        public float characterDirection { get; set; }
        [SerializeField]
        public PlayerState playerState;
        [SerializeField]
        public float playerMoveSpeed= 10f;
        [SerializeField]
        public float jumpSpeed = 300f;
        [SerializeField]
        public float dashTimer = 0f;
        [SerializeField]
        public float maxDashDistance = 1.5f;
        [SerializeField]
        public float dashForce= 10f;
        [SerializeField]
        public float dashCooldown = 1f;



        public Player(Rigidbody2D _rb ,PlayerCollisionHelper _playerCollisionHelper, SpriteRenderer _playerSprite, BoxCollider2D _playerCollider)
        {
            playerCollisionHelper = _playerCollisionHelper;
            playerSprite = _playerSprite;
            rb = _rb;
            characterDirection = 1f;
            playerSprite.flipX = true;
            dashState = DashState.READY;
            playerState = PlayerState.GROUNDED;
            pc = _playerCollider;
        }

        public void Jump()
        {
            rb.AddRelativeForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        }

        public void Jumping()
        {
            Vector3 vel = rb.velocity;
            vel.y -= 1 * Time.deltaTime;
            rb.velocity = vel;
        }

        public void MovePlayer(Vector2 movement)
        {
            rb.velocity = new Vector3(movement.x * playerMoveSpeed, rb.velocity.y, 0);
        }

        public void WallJump()
        {

        }

        public bool WallInFrontOfPlayer()
        {
            return playerCollisionHelper.WallInFrontOfMe(rb, pc, characterDirection);
        }

        public void WallCollisionResolver(Vector2 playerInput)
        {
            if (dashState == DashState.DASHING)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                dashSavedVelocity = new Vector2(0, 0);
            }
            if (playerState == PlayerState.JUMPING || playerState == PlayerState.WALL_GRINDING)
            {
                if (characterDirection > 0 && playerInput.x > 0)
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, 0);

                    playerState = PlayerState.WALL_GRINDING;
                }
                else if (characterDirection < 0 && playerInput.x < 0)
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

        public void EndingWallGrind()
        {
            rb.gravityScale = 1;
            if (!playerCollisionHelper.IsPLayerGrounded(rb, pc))
            {
                playerState = PlayerState.JUMPING;
            }
            else
            {
                playerState = PlayerState.GROUNDED;
            }
        }

        public void CheckIfPLayerGrounded()
        {
            if (!playerCollisionHelper.IsPLayerGrounded(rb, pc) && playerState != PlayerState.WALL_GRINDING)
            {
                playerState = PlayerState.JUMPING;
            }
            else if (playerState != PlayerState.WALL_GRINDING)
            {
                playerState = PlayerState.GROUNDED;
            }
        }

        public void FlipPlayerSprite()
        {
            if (rb.velocity.x > 0.01f && !playerSprite.flipX)
            {
                playerSprite.flipX = true;
                characterDirection = 1f;
            }
            if (rb.velocity.x < -0.01f && playerSprite.flipX)
            {
                playerSprite.flipX = false;
                characterDirection = -1f;
            }
        }

        public void TryDash()
        {
            switch (dashState)
            {
                case DashState.READY:
                    StartDash();
                    break;
                case DashState.DASHING:
                    Dashing();
                    break;
                case DashState.COOLDOWN:
                    DashCooldown();
                    break;
            }
        }

        public void StartDash()
        {
            dashSavedVelocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = new Vector2(characterDirection * dashForce, 0);
            dashState = DashState.DASHING;
            rb.isKinematic = true;
        }

        public void Dashing()
        {
            dashTimer += Time.deltaTime * 3;
            if (dashTimer >= maxDashDistance)
            {
                rb.isKinematic = false;
                dashTimer = 0f;
                rb.velocity = dashSavedVelocity;
                dashState = DashState.COOLDOWN;
            }
        }
        public void DashCooldown()
        {
            dashCooldown -= Time.deltaTime;
            if (dashCooldown <= 0)
            {
                dashCooldown = 1f;
                dashState = DashState.READY;
            }
        }
    }
}
