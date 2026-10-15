using System.Collections.Generic;

public readonly struct ExposureTimingResult
{
    public float ExposureMilliseconds { get; }
    public ShotSample Shot { get; }

    public ExposureTimingResult(
        float exposureMilliseconds,
        ShotSample shot
    )
    {
        ExposureMilliseconds = exposureMilliseconds;
        Shot = shot;
    }
}

public static class ExposureTimingCalculator
{
    public static bool TryCalculate(
        bool hasVisibilityResult,
        float visibleAtSeconds,
        IReadOnlyList<ShotSample> shots,
        out ExposureTimingResult result
    )
    {
        result = default;

        if (!hasVisibilityResult || shots == null)
        {
            return false;
        }

        bool foundEligibleShot = false;
        ShotSample earliestEligibleShot = default;

        foreach (ShotSample shot in shots)
        {
            bool isEligible =
                shot.Time >= visibleAtSeconds;

            bool isEarlier =
                !foundEligibleShot ||
                shot.Time < earliestEligibleShot.Time;

            if (isEligible && isEarlier)
            {
                earliestEligibleShot = shot;
                foundEligibleShot = true;
            }
        }

        if (!foundEligibleShot)
        {
            return false;
        }

        float exposureMilliseconds =
            (earliestEligibleShot.Time - visibleAtSeconds) *
            1000f;

        result = new ExposureTimingResult(
            exposureMilliseconds,
            earliestEligibleShot
        );

        return true;
    }
}
