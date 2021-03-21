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
        PlayerDto dto;
        [SerializeField]
        public DashState dashState;
        [SerializeField]
        public PlayerState playerState;
        public float timeCounterTemp;
        [SerializeField]
        public float characterDirection { get; set; }
        private float dashForcetemp;

        public Player(Rigidbody2D _rb ,PlayerCollisionHelper _playerCollisionHelper, SpriteRenderer _playerSprite, BoxCollider2D _playerCollider, PlayerDto _dto)
        {
            playerCollisionHelper = _playerCollisionHelper;
            playerSprite = _playerSprite;
            rb = _rb;
            dto = _dto;
            characterDirection = 1f;
            playerSprite.flipX = true;
            dashState = DashState.READY;
            playerState = PlayerState.GROUNDED;
            pc = _playerCollider;
            timeCounterTemp = dto.jumpTimeCounter;
            dashForcetemp = dto.dashForce;
        }

        public bool Jump()
        {
            if (timeCounterTemp > 0 || timeCounterTemp > dto.minJumpTime)
            {
                rb.velocity = new Vector2(rb.velocity.x, 1f * dto.jumpSpeed);
                //rb.AddRelativeForce(Vector2.up * dto.jumpSpeed, ForceMode2D.Impulse);
                timeCounterTemp -= Time.deltaTime;
                if (rb.gravityScale < dto.gravityFallMax)
                {
                    rb.gravityScale += 0.1f;
                }
                return true;
            }
            playerState = PlayerState.FALLING;
            return false;
        }

        public void Falling()
        {
            if (rb.gravityScale < dto.gravityFallMax)
            {
                rb.gravityScale += 0.1f;
            }
        }

        public void MovePlayerWhileJumping(Vector2 movement)
        {
            rb.velocity = new Vector3(movement.x * dto.playerMoveSpeedInAir, rb.velocity.y, 0);
        }

        public void MovePlayer(Vector2 movement)
        {
            rb.velocity = new Vector3(movement.x * dto.playerMoveSpeed, rb.velocity.y, 0);
        }

        public void WallJump()
        {
            rb.velocity = new Vector2(characterDirection * -1, 1f * dto.jumpSpeed);
            timeCounterTemp = dto.jumpTimeCounter;
            playerState = PlayerState.JUMPING;
            EndWallGrind();
        }

        public bool WallInFrontOfPlayer()
        {
            return playerCollisionHelper.GroundOrWallInFrontOfMe(rb, pc, characterDirection);
        }

        public void WallCollisionResolver(Vector2 playerInput)
        {
            if (dashState == DashState.DASHING)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                dashSavedVelocity = new Vector2(0, 0);
                EndDash();
            }
            if (playerState == PlayerState.JUMPING || playerState == PlayerState.WALL_GRINDING || playerState == PlayerState.FALLING)
            {
                if ((characterDirection > 0 && playerInput.x > 0) 
                    || (characterDirection < 0 && playerInput.x < 0))
                {
                    rb.gravityScale = 0;
                    rb.velocity = new Vector2(0, 0);
                    playerState = PlayerState.WALL_GRINDING;
                }
                else
                {
                    rb.gravityScale = dto.gravityScale;
                }
            }
        }

        public void EndingWallGrind()
        {
            rb.gravityScale = dto.gravityScale;
            if (!playerCollisionHelper.IsPLayerGrounded(rb, pc))
            {
                playerState = PlayerState.FALLING;
            }
            else
            {
                GroundPLayer();
            }
        }

        public void CheckIfPLayerGrounded()
        {
            if (!playerCollisionHelper.IsPLayerGrounded(rb, pc))
            {
                if (playerState != PlayerState.WALL_GRINDING && playerState != PlayerState.JUMPING)
                    playerState = PlayerState.FALLING;
            }
            else if (playerState == PlayerState.FALLING)
            {
                GroundPLayer();
                timeCounterTemp = dto.jumpTimeCounter;
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
            rb.gravityScale = 0;
            rb.mass = 0;
            rb.velocity = new Vector2(characterDirection * dashForcetemp, 0);
            dashState = DashState.DASHING;
        }

        public void Dashing()
        {
            dto.dashTimer += Time.deltaTime * 2;
            if (dto.dashTimer <= dto.maxDashDistance)
            {
                rb.velocity = new Vector2(characterDirection * dashForcetemp, 0);
                dashForcetemp -= 2f;
            }
            else
            {
               rb.velocity = dashSavedVelocity;
                EndDash();
            }
        }

        public void EndDash()
        {
            rb.gravityScale = dto.gravityScale;
            dto.dashTimer = 0f;
            rb.mass = 40;
            dashForcetemp = dto.dashForce;
            dashState = DashState.COOLDOWN;
        }

        private void EndWallGrind()
        {
            rb.gravityScale = dto.gravityScale;
        }

        public void DashCooldown()
        {
            dto.dashCooldown -= Time.deltaTime;
            if (dto.dashCooldown <= 0)
            {
                dto.dashCooldown = 1f;
                dashState = DashState.READY;
            }
        }

        private void GroundPLayer()
        {
            playerState = PlayerState.GROUNDED;
            rb.gravityScale = dto.gravityScale;
        }
    }
}
