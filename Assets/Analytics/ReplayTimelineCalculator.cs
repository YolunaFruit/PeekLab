using System.Collections.Generic;

public readonly struct ReplaySegment
{
    public int FromIndex { get; }
    public int ToIndex { get; }
    public float InterpolationAmount { get; }
    public bool IsFinished { get; }

    public ReplaySegment(
        int fromIndex,
        int toIndex,
        float interpolationAmount,
        bool isFinished
    )
    {
        FromIndex = fromIndex;
        ToIndex = toIndex;
        InterpolationAmount = interpolationAmount;
        IsFinished = isFinished;
    }
}

public static class ReplayTimelineCalculator
{
    public static bool TryCalculate(
        IReadOnlyList<float> frameTimes,
        float replayTime,
        out ReplaySegment segment
    )
    {
        segment = default;

        if (frameTimes == null || frameTimes.Count == 0)
        {
            return false;
        }

        int lastIndex = frameTimes.Count - 1;

        if (frameTimes.Count == 1 || replayTime >= frameTimes[lastIndex])
        {
            segment = new ReplaySegment(
                lastIndex,
                lastIndex,
                0f,
                true
            );

            return true;
        }

        if (replayTime <= frameTimes[0])
        {
            segment = new ReplaySegment(0, 1, 0f, false);
            return true;
        }

        int lowerBound = 0;
        int upperBound = lastIndex;

        while (lowerBound < upperBound)
        {
            int middle = lowerBound + (upperBound - lowerBound) / 2;

            if (frameTimes[middle] <= replayTime)
            {
                lowerBound = middle + 1;
            }
            else
            {
                upperBound = middle;
            }
        }

        int toIndex = lowerBound;
        int fromIndex = toIndex - 1;
        float segmentDuration =
            frameTimes[toIndex] - frameTimes[fromIndex];

        float interpolationAmount = segmentDuration > 0f
            ? (replayTime - frameTimes[fromIndex]) / segmentDuration
            : 1f;

        interpolationAmount = Clamp01(interpolationAmount);

        segment = new ReplaySegment(
            fromIndex,
            toIndex,
            interpolationAmount,
            false
        );

        return true;
    }

    private static float Clamp01(float value)
    {
        if (value < 0f)
        {
            return 0f;
        }

        return value > 1f ? 1f : value;
    }
}
