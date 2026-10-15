public readonly struct ShotSample
{
    public float Time { get; }
    public float MovementSpeed { get; }
    public bool WasStable { get; }
    public float StoppedForMilliseconds { get; }

    public ShotSample(
        float time,
        float movementSpeed,
        bool wasStable,
        float stoppedForMilliseconds
    )
    {
        Time = time;
        MovementSpeed = movementSpeed;
        WasStable = wasStable;
        StoppedForMilliseconds =
            stoppedForMilliseconds;
    }
}
