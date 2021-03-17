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
        private float collisionDistanceBelow = 0.77f;

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

        private bool GroundBelowMe(Rigidbody2D player)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - 1f), Vector2.down);
            if (hit.collider != null && hit.collider.name == "Ground")
            {
                float distance = Mathf.Abs(hit.point.y - (player.transform.position.y - 1f));
                if (distance <= collisionDistanceBelow)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsPLayerGrounded(Rigidbody2D player)
        {
            if (!GroundBelowMe(player))
            {
                return false;
            }
            return true;
        }
    }
}
