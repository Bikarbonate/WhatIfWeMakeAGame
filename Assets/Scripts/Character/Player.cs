using System;
using System.Collections;
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
        public State dashState;
        [SerializeField]
        public PlayerState playerState;
        public float timeCounterTemp;
        [SerializeField]
        public float characterDirection { get; set; }
        private float dashForcetemp;
        private float wallJumpTimerTemp;
        private PlayerState savedState;
        private float wallDirection;
        private Vector2 wallJumpVelocity;
        private float wallJumpTimer;


        public Player(Rigidbody2D _rb, PlayerCollisionHelper _playerCollisionHelper, SpriteRenderer _playerSprite, BoxCollider2D _playerCollider, PlayerDto _dto)
        {
            playerCollisionHelper = _playerCollisionHelper;
            playerSprite = _playerSprite;
            rb = _rb;
            dto = _dto;
            characterDirection = 1f;
            playerSprite.flipX = true;
            dashState = State.READY;
            playerState = PlayerState.GROUNDED;
            pc = _playerCollider;
            timeCounterTemp = 0f;
            dashForcetemp = dto.dashForce;
            wallJumpTimerTemp = 0f;
        }

        public bool Jump(bool shouldKeepJumping)
        {
            if (timeCounterTemp < dto.minJumpTime || (timeCounterTemp < dto.jumpTimeCounter && shouldKeepJumping))
            {
                rb.velocity = new Vector2(rb.velocity.x, 1f * dto.jumpSpeed);
                timeCounterTemp += Time.deltaTime;
                if (rb.gravityScale < dto.gravityFallMax)
                {
                    rb.gravityScale += 0.1f;
                }
                return true;
            }
            timeCounterTemp = 0f;
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

        public void StopPlayer()
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = 0;
        }

        public void StartWallJump(Vector2 playerInput)
        {
            if (playerInput.x == wallDirection)
            {
                wallJumpVelocity = new Vector2(wallDirection * -dto.wallJumpClimb.x, dto.wallJumpClimb.y);
                wallJumpTimer = dto.wallJumpClimbTimer;

            }
            else if (playerInput.x == 0)
            {
                wallJumpVelocity = new Vector2(wallDirection * -dto.wallJumpOff.x, dto.wallJumpOff.y);
                wallJumpTimer = dto.wallJumpLeapTimerOff;

            }
            else
            {
                wallJumpVelocity = new Vector2(wallDirection * -dto.wallJumpLeap.x, dto.wallJumpLeap.y);
                wallJumpTimer = dto.wallJumpLeapTimer;
            }
            playerState = PlayerState.WALL_JUMPING;
        }

        public bool WallInFrontOfPlayer()
        {
            if (playerCollisionHelper.GroundOrWallInFrontOfMe(rb, pc, characterDirection))
            {
                wallDirection = characterDirection;
                return true;
            }
            wallDirection = 0f;
            return false;
        }

        public void WallCollisionResolver(Vector2 playerInput)
        {
            if (dashState == State.DASHING)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                dashSavedVelocity = new Vector2(0, 0);
                EndDash();
            }

            if (playerState == PlayerState.JUMPING
                || playerState == PlayerState.WALL_GRINDING
                || playerState == PlayerState.FALLING
                || playerState == PlayerState.WALL_SLIDING)
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
                    if (rb.velocity.y > dto.wallSlideSpeedMax)
                        rb.velocity = new Vector2(rb.velocity.x, dto.wallSlideSpeedMax);
                    playerState = PlayerState.WALL_SLIDING;
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
                if (playerState != PlayerState.WALL_GRINDING
                    && playerState != PlayerState.JUMPING
                    && playerState != PlayerState.WALL_JUMPING
                    && playerState != PlayerState.WALL_SLIDING)
                    playerState = PlayerState.FALLING;
            }
            else if (playerState != PlayerState.JUMPING)
            {
                GroundPLayer();
                timeCounterTemp = 0f;
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


        public void StartDash()
        {
            dashSavedVelocity = new Vector2(rb.velocity.x, 0);
            rb.gravityScale = 0;
            rb.mass = 0;
            rb.velocity = new Vector2(characterDirection * dashForcetemp, 0);
            savedState = playerState;
            playerState = PlayerState.DASHING;
            dashState = State.DASHING;
        }

        public void Dashing()
        {
            dto.dashTimer += Time.deltaTime * 2;
            if (dto.dashTimer <= dto.maxDashDistance)
            {
                playerState = PlayerState.DASHING;
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
            dashState = State.COOLDOWN;
            playerState = savedState;
        }

        public void DashCooldown()
        {
            dto.dashCooldown -= Time.deltaTime;
            if (dto.dashCooldown <= 0)
            {
                dto.dashCooldown = 1f;
                dashState = State.READY;
            }
        }

        private void GroundPLayer()
        {
            playerState = PlayerState.GROUNDED;
            rb.gravityScale = dto.gravityScale;
        }


        public IEnumerator WallJumpRoutine()
        {
            if (wallJumpTimerTemp < wallJumpTimer)
            {
                rb.velocity = wallJumpVelocity;
                wallJumpTimerTemp += Time.deltaTime;
                playerState = PlayerState.WALL_JUMPING;
                if (rb.gravityScale < dto.gravityFallMax)
                {
                    rb.gravityScale += 0.1f;
                }
                yield return null;
            }
            playerState = PlayerState.JUMPING;
            wallJumpTimerTemp = 0f;
            yield return null;
        }
    }
}
