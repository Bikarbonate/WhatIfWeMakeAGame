using System;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class PlayerDto
    {
        [SerializeField]
        public float playerMoveSpeed = 10f;
        [SerializeField]
        public float jumpSpeed = 300f;
        [SerializeField]
        public float dashTimer = 0f;
        [SerializeField]
        public float maxDashDistance = 1.5f;
        [SerializeField]
        public float dashForce = 10f;
        [SerializeField]
        public float dashCooldown = 1f;
        [SerializeField]
        public float jumpTimeCounter = 1f;
        [SerializeField]
        public float gravityFallMax;
        [SerializeField]
        public float playerMoveSpeedInAir;
        [SerializeField]
        public float minJumpTime;
        [SerializeField]
        public float gravityScale;
        [SerializeField]
        public float wallJumpTimer;
        [SerializeField]
        public float wallJumpForce;
    }
}
