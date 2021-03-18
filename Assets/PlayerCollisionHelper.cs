using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class PlayerCollisionHelper
    {
        private float extraLenght = 0.4f;
        private float extraheight = 0.1f;
        LayerMask groundMask;
        LayerMask wallMask;

        public PlayerCollisionHelper()
        {
            groundMask = LayerMask.GetMask("Ground");
             wallMask = LayerMask.GetMask("Wall");
        }
        public bool WallInFrontOfMe(Rigidbody2D player, BoxCollider2D pc, float characterDirection)
        {
            RaycastHit2D hit = Physics2D.BoxCast(pc.bounds.center, pc.bounds.size / 2, 0f, new Vector2(characterDirection, 0), extraLenght, wallMask);
            if (hit.collider != null)
            {
                return true;
            }
            return false;
        }

        private bool GroundBelowMe(Rigidbody2D player, BoxCollider2D pc)
        {
            RaycastHit2D hit = Physics2D.BoxCast(pc.bounds.center, pc.bounds.size, 0f, Vector2.down, extraheight, groundMask);
            if (hit.collider != null)
            {
                return true;
            }
            return false;
        }

        public bool IsPLayerGrounded(Rigidbody2D player, BoxCollider2D pc)
        {
            if (!GroundBelowMe(player, pc))
            {
                return false;
            }
            return true;
        }
    }
}
