using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets
{
    public enum DashState
    {
        READY = 1, 
        DASHING,
        COOLDOWN
    }

    public enum PlayerState
    {
        GROUNDED = 1,
        JUMPING,
        FALLING,
        WALL_GRINDING
    }
}
