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
        private float collisionDistanceFront = 0.8f;
        private float extraheight = 0.1f;

        public bool WallInFrontOfMe(Rigidbody2D player, float characterDirection)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, new Vector2(characterDirection, 0));
            if (hit.collider != null && hit.collider.name == "Wall")
            {
                float distance = Mathf.Abs(hit.point.x - player.transform.position.x);
                if (distance < collisionDistanceFront)
                {
                    return true;
                }
            }
            return false;
        }

        private bool GroundBelowMe(Rigidbody2D player, BoxCollider2D pc)
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.BoxCast(pc.bounds.center, pc.bounds.size, 0f, Vector2.down, extraheight, mask);
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
