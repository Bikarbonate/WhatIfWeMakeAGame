namespace Assets
{
    public enum State
    {
        READY = 1,
        COOLDOWN,
        DASHING,
    }

    public enum WallJumpState
    {
        JUMPOFF = 1,
        JUMPLEAP,
        JUMPCLIMB
    }


    public enum PlayerState
    {
        GROUNDED = 1,
        JUMPING,
        FALLING,
        WALL_GRINDING,
        WALL_JUMPING,
        WALL_SLIDING,
        DASHING
    }
}
