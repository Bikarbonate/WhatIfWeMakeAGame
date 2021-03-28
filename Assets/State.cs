namespace Assets
{
    public enum State
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
        WALL_GRINDING,
        WALL_JUMPING
    }
}
